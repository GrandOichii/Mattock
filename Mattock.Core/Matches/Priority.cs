using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers;

namespace Mattock.Core.Matches;

public class Priority
{
    public Match Match { get; }
    public int InitialPlayerIdx { get; private set; }
    public int NextPlayerIdx { get; private set; }
    public int PriorityPlayerIdx { get; private set; }
    public bool Done { get; private set; }

    public Priority(Match match)
    {
        Match = match;

        Reset(match.TurnManager.ActivePlayerIdx);
    }

    public void Reset(int initialPlayerIdx)
    {
        InitialPlayerIdx = initialPlayerIdx;
        PriorityPlayerIdx = initialPlayerIdx;
        CalculateNext();
        Done = false;
    }

    public async Task<RollbackRequest?> Resolve()
    {
        while (!Done && !Match.ShouldHalt())
        {
            var rollback = await ProcessPriority(Match.Players[PriorityPlayerIdx]);
            if (rollback is not null)
                return rollback;
        }
        return null;
    }

    private async Task<RollbackRequest?> ProcessPriority(Player player)
    {
        Match.StateBasedActions.Apply();
        if (player.Status == PlayerStatus.Lost)
        {
            Advance();
            return null;
        }
        if (Match.ShouldHalt())
        {
            return null;
        }

        var abilities = Match.Triggers.PopTriggeredAbilityQueue();

        // TODO order (603.3b)
        QueuedTriggeredAbility[] ordered = [.. abilities];
        RollbackRequest? rollback;

        foreach (var ability in ordered)
        {
            rollback = await Match.Events.TriggerAbility(ability);
            if (rollback is not null)
                return rollback;
        }

        ICommand command;
        (command, rollback) = await player.PromptCommand();
        if (rollback is not null)
            return rollback;

        rollback = await command.Do();
        if (rollback is not null)
            return rollback;
        return null;
    }

    public void Advance()
    {
        if (Match.ShouldHalt()) return;
        CalculateCurrent();
        CalculateNext();
    }

    private void CalculateCurrent()
    {
        PriorityPlayerIdx = NextPlayerIdx;
        Done = PriorityPlayerIdx == InitialPlayerIdx;
    }

    private void CalculateNext()
    {
        NextPlayerIdx = Match.TurnManager.NextInTurnOrderIdx(PriorityPlayerIdx);
    }
}
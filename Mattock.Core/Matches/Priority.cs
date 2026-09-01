using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

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
        var (command, rollback) = await player.PromptCommand();
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
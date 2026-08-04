using System.Diagnostics;
using Mattock.Core.Matches.Players;

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

    public async Task Resolve()
    {
        while (!Done && !Match.AreWinnersDecided())
        {
            await ProcessPriority(Match.Players[PriorityPlayerIdx]);
        }
    }

    private async Task ProcessPriority(Player player)
    {
        Match.StateBasedActions.Apply();
        if (player.Status == PlayerStatus.Lost)
        {
            Advance();
            return;
        }
        if (Match.AreWinnersDecided())
        {
            return;
        }
        var (command, rollback) = await player.PromptCommand();
        if (rollback is not null)
        {
            throw new Exception($"Approved rollback request from player {player.GetDisplayName()} to snapshot with id = {rollback.RequestedSnapshotId}");
            return;
        }
        await command.Do();
    }

    public void Advance()
    {
        if (Match.AreWinnersDecided()) return;
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
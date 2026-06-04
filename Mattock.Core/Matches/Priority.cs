using System.Diagnostics;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches;

public class Priority
{
    public Match Match { get; }
    public int InitialPlayerIdx { get; private set; }
    public int NextPlayerIdx { get; private set; }
    public int PriorityPlayerIdx { get; private set; }

    public Priority(Match match)
    {
        Match = match;

        InitialPlayerIdx = match.TurnManager.ActivePlayerIdx;
        Reset();
    }

    public void Reset()
    {
        PriorityPlayerIdx = InitialPlayerIdx;
        CalculateNext();
    }

    public async Task Resolve()
    {
        // TODO make this better
        while(NextPlayerIdx != InitialPlayerIdx)
        {
            await ProcessPriority(Match.Players[PriorityPlayerIdx]);
        } 
        
        await ProcessPriority(Match.Players[PriorityPlayerIdx]);
    }

    private async Task ProcessPriority(Player player)
    {
        Match.StateBasedActions.Apply();
        var command = await player.PromptCommand();
        await command.Do();
    }

    public void Advance()
    {
        CalculateCurrent();
        CalculateNext();
    }

    private void CalculateCurrent()
    {
        PriorityPlayerIdx = NextPlayerIdx;
    }

    private void CalculateNext()
    {
        NextPlayerIdx = Match.TurnManager.NextInTurnOrderIdx(PriorityPlayerIdx);
    }
}
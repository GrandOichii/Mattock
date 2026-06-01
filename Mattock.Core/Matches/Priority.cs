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

        InitialPlayerIdx = match.TurnOrderManager.ActivePlayerIdx;
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
            var priorityPlayer = Match.Players[PriorityPlayerIdx];
            var command = await priorityPlayer.PromptCommand();
            await command.Do();
        } 
        
        
        var pp = Match.Players[PriorityPlayerIdx];
        var c = await pp.PromptCommand();
        await c.Do();
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
        NextPlayerIdx = Match.TurnOrderManager.NextInTurnOrderIdx(PriorityPlayerIdx);
    }
}
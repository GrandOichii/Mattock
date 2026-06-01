namespace Mattock.Core.Matches.Turns;

public class TurnOrderManager(Match match)
{
    public int ActivePlayerIdx { get; set; } = -1;

    public int NextInTurnOrderIdx(int playerIdx)
    {
        return (playerIdx + 1) % match.Players.Length;
    }

    public void Advance()
    {
        // TODO
        ActivePlayerIdx = NextInTurnOrderIdx(ActivePlayerIdx);
    }
}
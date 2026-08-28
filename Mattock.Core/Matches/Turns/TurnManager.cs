using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(
    Match _match
)
{
    public int ActivePlayerIdx { get; set; } = -1;
    public int TurnCounter { get; set; } = 0;
    public TurnResolver? Turn = null;

    public int NextInTurnOrderIdx(int playerIdx)
    {
        int result = playerIdx;
        Player player;
        do
        {
            result = (result + 1) % _match.Players.Length;
            player = _match.Players[result];
        }
        while (!player.IsInGame());

        return result;
    }

    public TurnResolver CreateTurn()
    {
        ++TurnCounter;
        return new(_match);
    }

    public void AdvanceTurn()
    {
        if (_match.ShouldHalt()) return;

        // TODO implement extra turns

        ActivePlayerIdx = NextInTurnOrderIdx(ActivePlayerIdx);
    }

    public async Task<RollbackRequest?> DoTurn()
    {
        Turn ??= CreateTurn();

        var request = await Turn.Resolve();
        if (request is not null)
            return request;
        Turn = null;

        AdvanceTurn();

        return null;
    }
}
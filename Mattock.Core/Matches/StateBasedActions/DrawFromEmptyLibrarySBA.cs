namespace Mattock.Core.Matches.StateBasedActions;

// 704.5b If a player attempted to draw a card from a library with no cards in it since the last time state-based actions were checked, that player loses the game.

public class DrawFromEmptyLibrarySBA : IStateBasedAction
{
    public bool Apply(Match match)
    {
        var result = false;
        foreach (var player in match.Players)
        {
            if (!player.DrewFromEmptyLibrary) continue;

            throw new NotImplementedException();
        }
        return result;
    }
}
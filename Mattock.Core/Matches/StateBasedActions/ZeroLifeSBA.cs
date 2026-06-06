using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.StateBasedActions;

// 104.3b If a player’s life total is 0 or less, that player loses the game the next time a player would receive priority. (This is a state-based action. See rule 704.)

public class ZeroLifeSBA : IStateBasedAction
{
    public bool Apply(Match match)
    {
        var result = false;
        foreach (var player in match.Players)
        {
            if (player.Life.Current > 0 || !match.Config.GameLossIfZeroOrLessLife) continue;

            player.SetStatus(PlayerStatus.Lost);
        }
        match.AreWinnersDecided();
        return result;
    }
}
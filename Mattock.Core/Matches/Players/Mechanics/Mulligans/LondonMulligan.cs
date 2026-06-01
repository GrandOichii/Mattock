using System.Threading.Tasks;

namespace Mattock.Core.Matches.Players.Mechanics.Mulligans;

public class LondonMulliganRule(
    int freeMulligans = 0,
    int decreasePerMulligan = 1
) : IMulliganRule
{
    public async Task Do(Player player, MulliganFrame frame)
    {
        var target = 0;
        if (frame.MulligansTaken >= freeMulligans) 
            target = (frame.MulligansTaken - freeMulligans + 1) * decreasePerMulligan;

        player.ShuffleHandIntoLibrary();
        player.Draw(player.Match.Config.InitialHandSize);

        for (; target > 0; --target)
        {
            var choice = await player.ChooseCard([.. player.Hand.Cards], $"Choose a card to up on the bottom of your library ({target} left)");

            player.Match.MoveCard(
                choice,
                player.Library,
                CardZoneChangeType.Bottom
            );
        }
    }
}
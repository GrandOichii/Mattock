using System.Threading.Tasks;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Zones;

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

        await player.ShuffleHandIntoLibrary();
        await player.Draw(player.Match.Config.InitialHandSize);

        for (; target > 0; --target)
        {
            var (choice, rollback) = await player.ChooseCard([.. player.Hand.Cards], $"Choose a card to up on the bottom of your library ({target} left)", false);
            if (rollback is not null)
                throw new MatchException($"Player {player.GetDisplayName()} requested rollback while doing a London mulligan");

            // TODO ignored rollback
            await player.Match.MoveCard(
                choice!,
                CardZoneChangeType.Bottom,
                player.Library.GetCardZoneChanger()
            );
        }
    }
}
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
            var (choices, rollback) = await player.ChooseCards([.. player.Hand.Cards], 1, 1, $"Choose a card to up on the bottom of your library ({target} left)");
            if (rollback is not null)
                throw new MatchException($"Player {player.GetDisplayName()} requested rollback while doing a London mulligan");

            if (choices.Length != 1)
                throw new CodeErrorException($"Got more than 1 card for moving card to bottom for {nameof(LondonMulliganRule)}");

            // TODO ignored rollback
            await player.Match.MoveCards([
                new(
                    choices[0],
                    CardZoneChangeType.Bottom,
                    player.Library.GetCardZoneChanger()
                )
            ]);
        }
    }
}
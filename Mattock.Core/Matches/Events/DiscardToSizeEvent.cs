using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

// TODO add random
public class DiscardToSize(
    Player player,
    int size
)
{
    public async Task<RollbackRequest?> Do()
    {
        List<Card> discarded = [];
        while (player.Hand.GetCount() > size)
        {
            var (cards, rollback) = await player.ChooseCards([.. player.Hand.Cards], 1, 1, "Discard cards to hand size");
            if (rollback is not null)
                return rollback;

            Card[] newDiscarded;
            (newDiscarded, rollback) = await player.Discard(cards);
            discarded.AddRange(newDiscarded);
            
            if (rollback is not null)
                return rollback;
        }

        foreach (var card in discarded)
        {
            player.Match.Triggers.Process(new(
                Triggers.TriggerType.SingleDiscard,
                new SingleDiscardTriggerContext(player, card)
            ));
        }

        // TODO ManyDiscard
        // TODO figure out order (if there is any)
        
        return null;
    }
}

public class DiscardToSizeEvent(
    DiscardToSize[] draws
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        foreach (var discard in draws)
        {
            var rollback = await discard.Do();
            if (rollback is not null)
                return rollback;
        }

        // TODO trigger
        
        return null;
    }
}
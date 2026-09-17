using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

public class Discard(
    Player player,
    int amount,
    bool random
)
{
    private async Task<(Card[], RollbackRequest?)> ChooseRandom()
    {
        Card[] cards = [.. player.Hand.Cards];
        
        return (
            [.. cards.OrderBy(i => player.Match.Rng.Next()).Take(amount)],
            null
        );
    }
    
    private async Task<(Card[], RollbackRequest?)> ChooseNonRandom()
    {
        return await player.ChooseCards(
            [.. player.Hand.Cards],
            amount, amount,
            $"Choose {amount} cards to discard"
        );
    }

    public async Task<RollbackRequest?> Do()
    {
        var (cards, rollback) = random
            ? await ChooseRandom()
            : await ChooseNonRandom();
        if (rollback is not null)
            return rollback;
            
        Card[] discarded;
        (discarded, rollback) = await player.Discard(cards);
        if (rollback is not null)
            return rollback;
        
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

public class DiscardEvent(
    Discard[] draws
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
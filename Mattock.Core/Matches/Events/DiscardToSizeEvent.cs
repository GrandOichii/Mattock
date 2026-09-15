using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

// TODO add random
public class DiscardToSize(
    Player player,
    int size
)
{
    public async Task<RollbackRequest?> Do()
    {
        while (player.Hand.GetCount() > size)
        {
            var (cards, rollback) = await player.ChooseCards([.. player.Hand.Cards], 1, 1, "Discard cards to hand size");
            if (rollback is not null)
                return rollback;

            rollback = await player.Discard(cards);
            if (rollback is not null)
                return rollback;
        }
        
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
using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

public class CardDraw(
    Player player,
    int amount
)
{
    public async Task<(Card[], RollbackRequest?)> Do()
    {
        return await player.Draw(amount);
    }

    public Player GetPlayer()
        => player;
}

public class CardDrawEvent(
    CardDraw[] draws
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        List<(Card, Player)> drawn = [];
        foreach (var draw in draws)
        {
            var (newDrawn, rollback) = await draw.Do();
            if (rollback is not null)
                return rollback;

            drawn.AddRange(newDrawn.Select(c => (c, draw.GetPlayer())));
        }

        foreach (var (card, player) in drawn)
        {
            match.Triggers.Process(new(
                TriggerType.SingleDraw,
                new SingleDrawTriggerContext(player, card)
            ));
        }

        // TODO ManyDraw
        // TODO figure out order (if there is any)
        
        return null;
    }
}
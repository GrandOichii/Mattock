using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class CardDraw(
    Player player,
    int amount
)
{
    public async Task<RollbackRequest?> Do()
    {
        return await player.Draw(amount);
    }
}

public class CardDrawEvent(
    CardDraw[] draws
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        foreach (var draw in draws)
        {
            var rollback = await draw.Do();
            if (rollback is not null)
                return rollback;
        }

        // TODO trigger
        
        return null;
    }
}
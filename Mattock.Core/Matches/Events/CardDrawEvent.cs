using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class CardDraw(
    Player player,
    int amount
)
{
    public void Do()
    {
        player.Draw(amount);
    }
}

public class CardDrawEvent(
    CardDraw[] draws
) : IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        foreach (var draw in draws)
        {
            draw.Do();
        }

        // TODO trigger
        
        return Task.FromResult<RollbackRequest?>(null);
    }
}
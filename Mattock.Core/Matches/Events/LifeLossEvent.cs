using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class LifeLoss(
    Player player,
    int amount
)
{
    public void Do()
    {
        player.Life.Lose(amount);
    }
}

public class LifeLossEvent(
    LifeLoss[] losses
) : IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        foreach (var loss in losses)
        {
            loss.Do();
        }

        // TODO trigger
        
        return Task.FromResult<RollbackRequest?>(null);
    }
}
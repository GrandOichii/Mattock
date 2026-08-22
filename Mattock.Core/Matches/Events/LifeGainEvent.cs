using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class LifeGain(
    Player player,
    int amount
)
{
    public void Do()
    {
        player.Life.Gain(amount);
    }
}

public class LifeGainEvent(
    LifeGain[] gains
) : IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        foreach (var gain in gains)
        {
            gain.Do();
        }

        // TODO trigger
        
        return Task.FromResult<RollbackRequest?>(null);
    }
}
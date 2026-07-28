using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Events;

public class LifeGain(
    Player player,
    int amount
)
{
    public void Do()
    {
        player.Life.Gain(amount);
        // player.Draw(amount);
    }
}

public class LifeGainEvent(
    LifeGain[] gains
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var gain in gains)
        {
            gain.Do();
        }

        // TODO trigger
        
        return Task.CompletedTask;
    }
}
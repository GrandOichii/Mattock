using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Triggers;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Events;

public class LifeGain(
    Player player,
    int amount
)
{
    public (Player, int) Do()
    {
        var was = player.Life.Current;
        player.Life.Gain(amount);
        return (player, player.Life.Current - was);
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
            var (player, gained) = gain.Do();
            if (gained <= 0) continue;

            match.Triggers.Process(new Trigger(
                TriggerType.LifeGain,
                new LifeGainTriggerContext(
                    player,
                    gained
                )
            ));
        }

        return Task.FromResult<RollbackRequest?>(null);
    }
}
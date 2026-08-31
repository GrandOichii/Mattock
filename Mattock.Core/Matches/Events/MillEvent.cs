using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class Mill(
    Player player,
    int amount
)
{
    public async Task<RollbackRequest?> Do()
    {
        return await player.Mill(amount);
    }
}

public class MillEvent(
    Mill[] mills
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        foreach (var mill in mills)
        {
            var request = await mill.Do();
            if (request is not null)
                return request;
        }

        // TODO trigger
        
        return null;
    }
}
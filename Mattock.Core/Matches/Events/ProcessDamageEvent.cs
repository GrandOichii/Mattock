using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class ProcessDamageEvent(
    Damage.Damage[] assignments
) : IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        foreach (var assignment in assignments)
        {
            assignment.Do();
        }

        // TODO trigger
        
        return Task.FromResult<RollbackRequest?>(null);
    }
}
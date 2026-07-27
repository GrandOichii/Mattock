using Mattock.Core.Matches.Damage;

namespace Mattock.Core.Matches.Events;

public class ProcessDamageEvent(
    DamageAssignment[] assignments
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var assignment in assignments)
        {
            assignment.Do();
        }

        // TODO trigger
        
        return Task.CompletedTask;
    }
}
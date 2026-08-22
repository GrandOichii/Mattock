using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class RemoveFromCombatEvent(
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        foreach (var permanent in match.Battlefield.GetInCombatPermanents())
        {
            await permanent.RemoveFromCombat();
        }

        // TODO trigger

        return null;
    }
}
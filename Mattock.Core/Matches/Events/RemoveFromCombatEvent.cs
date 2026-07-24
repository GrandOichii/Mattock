namespace Mattock.Core.Matches.Events;

public class RemoveFromCombatEvent(
) : IEvent
{
    public async Task Do(Match match)
    {
        foreach (var permanent in match.Battlefield.GetInCombatPermanents())
        {
            await permanent.RemoveFromCombat();
        }

        // TODO trigger
    }
}
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Ending;

public class CleanupStep(
    Phase phase
) : Step(
    phase,
    StepType.Cleanup,
    [
        new CleanupStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}

public class CleanupStepPart
    : IStepPart
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        // 514.1. Discard to max hand size
        var player = match.GetActivePlayer();
        var maxHandSize = player.GetMaxHandSize();


        if (maxHandSize is not null)
        {
            var rollback = await match.Events.DiscardToSize([ new(player, (int)maxHandSize) ]);
            if (rollback is not null)
                return rollback;
        }

        // 514.2. Remove all marked damage
        foreach (var permanent in match.Battlefield.GetPermanents())
        {
            permanent.RemoveMarkedDamage();
        }

        // 514.3. Priority (if any effects on the stack)
        // TODO

        // 514.3a State-based actions
        match.StateBasedActions.Apply();

        return null;
    }
}
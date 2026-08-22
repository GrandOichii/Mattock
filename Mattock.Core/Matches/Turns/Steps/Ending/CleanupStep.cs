using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Ending;

public class CleanupStep(
    Phase phase
) : Step(phase, StepType.Cleanup, false)
{
    public override async Task<RollbackRequest?> DoPrePriority()
    {
        // 514.1. Discard to max hand size
        var player = Match.GetActivePlayer();
        var maxHandSize = player.GetMaxHandSize();
        if (maxHandSize is not null)
        {
            while (player.Hand.GetCount() > maxHandSize)
            {
                var (card, rollback) = await player.ChooseCard([.. player.Hand.Cards], "Discard cards to hand size", false);
                if (rollback is not null)
                    return rollback;
                player.Discard([card!]);
            }
            // TODO
        }

        // 514.2. Remove all marked damage
        foreach (var permanent in Match.Battlefield.GetPermanents())
        {
            permanent.RemoveMarkedDamage();
        }

        // 514.3. Priority (if any effects on the stack)
        // TODO

        // 514.3a State-based actions
        Match.StateBasedActions.Apply();

        return null;
    }

    
    public override Task<RollbackRequest?> DoPostPriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override bool CanBeTaken() => true;

}
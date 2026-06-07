using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Ending;

public class CleanupStep : Step
{
    public CleanupStep(Phase phase) : base(
        phase, 
        StepType.Cleanup, 
        false
    )
    {
    }

    public override async Task DoPrePriority()
    {
        // 514.1. Discard to max hand size
        var player = Match.GetActivePlayer();
        var maxHandSize = player.GetMaxHandSize();
        if (maxHandSize is not null)
        {
            while (player.Hand.GetCount() > maxHandSize)
            {
                var card = await player.ChooseCard([.. player.Hand.Cards], "Discard cards to hand size");
                player.Discard([card]);
            }
            // TODO
        }

        // 514.2. Remove all marked damage
        // TODO

        // 514.3. Priority (if any effects on the stack)
        // TODO

        // 514.3a State-based actions
        Match.StateBasedActions.Apply();
    }

    
    public override Task DoPostPriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    public override bool CanBeTaken() => true;

}
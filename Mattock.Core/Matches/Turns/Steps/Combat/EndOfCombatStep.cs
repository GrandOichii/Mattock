
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class EndOfCombatStep(
    Phase phase
) : Step(phase, StepType.EndOfCombat, true)
{
    public override Task DoPrePriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    
    public override async Task DoPostPriority()
    {
        await Match.Events.RemoveAllFromCombat();
    }

    public override bool CanBeTaken() => true;

}

using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class BeginningOfCombatStep : Step
{
    public BeginningOfCombatStep(Phase phase) : base(phase, StepType.BeginningOfCombat, true)
    {
    }

    public override Task DoPrePriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    
    public override Task DoPostPriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    public override bool CanBeTaken() => true;

}
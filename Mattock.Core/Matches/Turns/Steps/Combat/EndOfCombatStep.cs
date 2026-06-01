
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class EndOfCombatStep : Step
{
    public EndOfCombatStep(Phase phase) : base(phase, StepType.EndOfCombat, true)
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
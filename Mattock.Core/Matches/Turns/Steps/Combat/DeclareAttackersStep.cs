
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareAttackersStep : Step
{
    public DeclareAttackersStep(Phase phase) : base(phase, StepType.DeclareAttackers, true)
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
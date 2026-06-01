
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Ending;

public class EndStep : Step
{
    public EndStep(Phase phase) : base(
        phase, 
        StepType.End, 
        true
    )
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
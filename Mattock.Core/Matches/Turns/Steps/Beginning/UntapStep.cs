using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UntapStep : Step
{
    public UntapStep(Phase phase) : base(
        phase,
        StepType.Untap, 
        false
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
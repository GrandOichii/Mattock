using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UpkeepStep : Step
{
    public UpkeepStep(Phase phase) : base(
        phase,
        StepType.Upkeep,
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
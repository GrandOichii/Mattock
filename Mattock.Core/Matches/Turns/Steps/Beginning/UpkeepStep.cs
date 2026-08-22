using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UpkeepStep(
    Phase phase
) : Step(phase, StepType.Upkeep, true)
{
    public override Task<RollbackRequest?> DoPrePriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    
    public override Task<RollbackRequest?> DoPostPriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override bool CanBeTaken() => true;

}
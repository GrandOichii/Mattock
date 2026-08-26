using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UpkeepStep(
    Phase phase
) : Step(
    phase,
    StepType.Upkeep,
    [
        new PriorityStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}
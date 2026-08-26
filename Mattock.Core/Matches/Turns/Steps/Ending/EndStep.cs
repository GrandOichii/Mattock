
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Ending;

public class EndStep(
    Phase phase
) : Step(
    phase,
    StepType.End,
    [
        new PriorityStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}
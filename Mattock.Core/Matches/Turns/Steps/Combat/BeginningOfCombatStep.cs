using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class BeginningOfCombatStep(
    Phase phase
) : Step(
    phase,
    StepType.BeginningOfCombat,
    [
        new PriorityStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}
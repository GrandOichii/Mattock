
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class EndOfCombatStep(
    Phase phase
) : Step(
    phase,
    StepType.EndOfCombat,
    [
        new PriorityStepPart(),
        new EndOfCombatStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}

public class EndOfCombatStepPart
    : IStepPart
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        return await match.Events.RemoveAllFromCombat();
    }
}
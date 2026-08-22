
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class EndOfCombatStep(
    Phase phase
) : Step(phase, StepType.EndOfCombat, true)
{
    public override Task<RollbackRequest?> DoPrePriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override async Task<RollbackRequest?> DoPostPriority()
    {
        return await Match.Events.RemoveAllFromCombat();
    }

    public override bool CanBeTaken() => true;

}
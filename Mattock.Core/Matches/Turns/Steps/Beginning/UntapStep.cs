using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UntapStep(
    Phase phase
) : Step(
    phase,
    StepType.Untap,
    [
        new UntapStepPart(),
    ]
)
{
    public override bool CanBeTaken() => true;
}

public class UntapStepPart
    : IStepPart
{
    public Task<RollbackRequest?> Do(Match match)
    {
        var active = match.GetActivePlayer();
        foreach (var p in match.Battlefield.GetPermanentsControlledBy(active))
        {
            p.HasSummoningSickness = false;
            // TODO untap
        }

        // TODO untap permanents

        return Task.FromResult<RollbackRequest?>(null);
    }
}
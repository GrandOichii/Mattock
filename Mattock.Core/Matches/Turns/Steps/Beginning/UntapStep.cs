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
        var permanents = match.Battlefield.GetPermanentsControlledBy(active);
        
        foreach (var p in permanents)
        {
            p.HasSummoningSickness = false;
            // TODO untap
        }

        return Task.FromResult<RollbackRequest?>(null);
    }
}
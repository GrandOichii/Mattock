using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class UntapStep(
    Phase phase
) : Step(
phase, StepType.Untap, false)
{
    public override Task<RollbackRequest?> DoPrePriority()
    {
        var active = Match.GetActivePlayer();
        foreach (var p in Match.Battlefield.GetPermanentsControlledBy(active))
        {
            p.HasSummoningSickness = false;
            // TODO untap
        }

        // TODO untap permanents

        return Task.FromResult<RollbackRequest?>(null);
    }

    public override Task<RollbackRequest?> DoPostPriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override bool CanBeTaken() => true;

}
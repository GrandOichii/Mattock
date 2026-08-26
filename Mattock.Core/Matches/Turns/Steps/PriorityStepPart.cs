using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Turns.Steps;

public class PriorityStepPart
    : IStepPart
{
    public async Task<RollbackRequest?> Do(Match _match)
    {
        // TODO? if the stack had resolved effects, does the active player still gain priority?

        var (_, r) = await _match.CreateAndResolvePriority();
        return r;
    }
}
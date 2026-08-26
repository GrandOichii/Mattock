using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Turns.Steps;

public interface IStepPart
{
    Task<RollbackRequest?> Do(Match match);
}
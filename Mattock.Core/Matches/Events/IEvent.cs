using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public interface IEvent
{
    Task<RollbackRequest?> Do(Match match);
}
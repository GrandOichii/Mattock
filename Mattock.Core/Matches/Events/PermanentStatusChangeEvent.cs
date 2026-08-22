using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Permanents.Statuses;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class PermanentStatusChangeEvent(
    Permanent[] _permanents,
    PermanentStatusType _type,
    bool _changeTo
): IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        List<Permanent> changed = [];
        foreach (var p in _permanents)
        {
            var status = p.GetStatus(_type);
            bool wasChanged = status.Set(_changeTo);
            if (!wasChanged) continue;
            changed.Add(p);
        }

        // TODO trigger
        return Task.FromResult<RollbackRequest?>(null);
    }
}
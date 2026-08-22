using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Events;

public class BlockDeclarationEvent(
    BlockDeclaration[] _declarations
) : IEvent
{
    public Task<RollbackRequest?> Do(Match match)
    {
        foreach (var dec in _declarations)
        {
            dec.Apply();
        }

        // TODO trigger
        return Task.FromResult<RollbackRequest?>(null);
    }
}
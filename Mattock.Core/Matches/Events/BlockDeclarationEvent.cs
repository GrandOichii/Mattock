using Mattock.Core.Matches.Combat;

namespace Mattock.Core.Matches.Events;

public class BlockDeclarationEvent(
    BlockDeclaration[] _declarations
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var dec in _declarations)
        {
            
        }

        // TODO trigger     
        return Task.CompletedTask;
    }
}
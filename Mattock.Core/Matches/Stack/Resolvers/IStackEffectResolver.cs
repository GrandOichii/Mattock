using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Stack.Resolvers;

public interface IStackEffectResolver
{
    Task<RollbackRequest?> Resolve(StackEffect effect);
    
    bool IsCard(Card card);
}
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Stack.Resolvers;

public interface IStackEffectResolver
{
    Task Resolve(StackEffect effect);
    bool IsCard(Card card);
}
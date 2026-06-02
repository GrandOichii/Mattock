using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Tests.Setup.Asserts;

public class SpellResolverAsserts(SpellResolver spell)
{
    public SpellResolverAsserts CardName(string name)
    {
        spell.Card.HasName(name).ShouldBeTrue();
        return this;
    }
}
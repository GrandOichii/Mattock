using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Tests.Setup.Asserts;


public class TriggeredAbilityResolverAsserts(TriggeredAbilityResolver ta)
{
    public TriggeredAbilityResolverAsserts CardName(string name)
    {
        ta.Ability.Card.HasName(name).ShouldBeTrue();
        return this;
    }
}
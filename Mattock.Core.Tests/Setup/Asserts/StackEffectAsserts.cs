using Mattock.Core.Matches.Stack;
using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Tests.Setup.Asserts;

public class StackEffectAsserts(StackEffect effect)
{
    public StackEffectAsserts HasController(int idx)
    {
        effect.Controller!.Idx.ShouldBe(idx);
        return this;
    }

    public StackEffectAsserts AssertAsSpell(Action<SpellResolverAsserts> action)
    {
        effect.Resolver.ShouldBeOfType<SpellResolver>();
        action(new((effect.Resolver as SpellResolver)!));
        return this;
    }
}
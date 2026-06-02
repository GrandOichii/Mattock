using Mattock.Core.Matches.Stack;

namespace Mattock.Core.Tests.Setup.Asserts;

public class StackAsserts(TheStack stack)
{
    public StackAsserts IsEmpty()
    {
        stack.IsEmpty().ShouldBeTrue();
        return this;
    }

    public StackAsserts EffectCount(int v)
    {
        stack.GetCount().ShouldBe(v);
        return this;
    }

    public StackAsserts AssertEffect(int idx, Action<StackEffectAsserts> action)
    {
        action(new(stack.Effects[idx]));
        return this;
    }
}
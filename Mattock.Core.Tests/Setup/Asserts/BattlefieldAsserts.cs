namespace Mattock.Core.Tests.Setup.Asserts;

public class BattlefieldAsserts(Battlefield battlefield)
{
    public BattlefieldAsserts HasPermanents(int count)
    {
        battlefield.GetPermanents().Length.ShouldBe(count);
        return this;
    }

    public BattlefieldAsserts AssertPermanent(int idx, Action<PermanentAsserts> action)
    {
        action(new(battlefield.GetPermanents()[idx]));
        return this;
    }

    public BattlefieldAsserts AssertPermanent(string name, Action<PermanentAsserts> action)
    {
        action(new(battlefield.GetPermanents().First(p => p.HasName(name))));
        return this;
    }
}
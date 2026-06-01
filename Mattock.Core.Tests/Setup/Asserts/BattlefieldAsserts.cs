namespace Mattock.Core.Tests.Setup.Asserts;

public class BattlefieldAsserts(Battlefield battlefield)
{
    public BattlefieldAsserts HasPermanents(int count)
    {
        battlefield.Permanents.Count.ShouldBe(count);
        return this;
    }

    public BattlefieldAsserts AssertPermanent(int idx, Action<PermanentAsserts> action)
    {
        action(new(battlefield.Permanents[idx]));
        return this;
    }

    public BattlefieldAsserts AssertPermanent(string name, Action<PermanentAsserts> action)
    {
        action(new(battlefield.Permanents.First(p => p.HasName(name))));
        return this;
    }
}
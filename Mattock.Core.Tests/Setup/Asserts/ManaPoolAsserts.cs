using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Tests.Setup.Asserts;

public class ManaPoolAsserts(ManaPool manaPool)
{
    public ManaPoolAsserts IsEmpty()
    {
        manaPool.IsEmpty().ShouldBeTrue();
        return this;
    }

    public ManaPoolAsserts HasTotalMana(int v)
    {
        manaPool.GetTotal().ShouldBe(v);
        return this;
    }
}
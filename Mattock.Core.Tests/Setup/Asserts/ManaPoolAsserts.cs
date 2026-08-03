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

    public ManaPoolAsserts HasColoredMana(ManaType t, int v)
    {
        manaPool.Mana.Count(m => m.Type == t).ShouldBe(v);
        return this;
    }
}
using Mattock.Core.Matches.Players.Cards.CardZones;

namespace Mattock.Core.Tests.Setup.Asserts;

public class PlayerAsserts(Player player)
{
    public PlayerAsserts HasLife(int v)
    {
        player.Life.Current.ShouldBe(v);
        return this;
    }

    public PlayerAsserts AssertLibrary(Action<LibraryAsserts> action)
    {
        action(new(player.Library));
        return this;
    }

    public PlayerAsserts AssertHand(Action<HandAsserts> action)
    {
        action(new(player.Hand));
        return this;
    }

    public PlayerAsserts AssertGraveyard(Action<GraveyardAsserts> action)
    {
        action(new(player.Graveyard));
        return this;
    }

    public PlayerAsserts AssertManaPool(Action<ManaPoolAsserts> action)
    {
        action(new(player.ManaPool));
        return this;
    }
}
using Mattock.Core.Matches.Players.Cards.CardZones;

namespace Mattock.Core.Tests.Setup.Asserts;

public class PlayerAsserts(Player player)
{
    public PlayerAsserts HasLife(int v)
    {
        player.Life.Current.ShouldBe(v);
        return this;
    }

    public PlayerAsserts AssertDeck(Action<DeckAsserts> action)
    {
        action(new(player.Deck));
        return this;
    }

    public PlayerAsserts AssertHand(Action<HandAsserts> action)
    {
        action(new(player.Hand));
        return this;
    }
}
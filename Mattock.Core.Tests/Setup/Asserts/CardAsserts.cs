using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Tests.Setup.Asserts;

public class CardAsserts(Card card)
{
    public CardAsserts HasName(string name)
    {
        card.HasName(name).ShouldBeTrue($"Card {card.GetDisplayName()} should have had name {name}, bu didn't");
        return this;
    }
}
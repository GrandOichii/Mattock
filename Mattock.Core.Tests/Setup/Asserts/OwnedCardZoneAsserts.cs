using Mattock.Core.Matches.Players.Cards.CardZones;

namespace Mattock.Core.Tests.Setup.Asserts;

public class OwnedCardZoneAsserts<T>(T zone) where T : OwnedCardZone
{
    public OwnedCardZoneAsserts<T> HasCardCount(int v)
    {
        zone.GetCount().ShouldBe(v, $"Card count of {nameof(OwnedCardZone)} of player {zone.Player.GetDisplayName()} should be {v}, but was {zone.GetCount()}");
        return this;
    }
}
using Mattock.Core.Matches.Players.Cards.CardZones;

namespace Mattock.Core.Tests.Setup.Asserts;

public class OwnedCardZoneAsserts<T>(T zone) where T : OwnedCardZone
{
    public OwnedCardZoneAsserts<T> HasCardCount(int v)
    {
        zone.GetCount().ShouldBe(v, $"Card count of {zone.GetZoneName()} of player {zone.Player.GetDisplayName()} should be {v}, but was {zone.GetCount()}");
        return this;
    }

    public OwnedCardZoneAsserts<T> IsEmpty()
    {
        zone.GetCount().ShouldBe(0, $"Card count of {typeof(T)} of player {zone.Player.GetDisplayName()} should be empty, but has {zone.GetCount()} cards");
        return this;
    }

    public OwnedCardZoneAsserts<T> AssertCard(int idx, Action<CardAsserts> action)
    {
        action(new(zone.Cards[idx]));
        return this;
    }
}
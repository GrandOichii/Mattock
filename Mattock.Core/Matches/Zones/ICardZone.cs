using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Zones;

public interface ICardZone
{
    string GetZoneName();

    void Remove(Card card);
}

public interface ICardZoneChanger
{
    string Do(Card card, CardZoneChangeType type);

    bool Accepts(Card card);
    
    ICardZone GetTargetZone();
}
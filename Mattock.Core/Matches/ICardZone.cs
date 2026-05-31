using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public interface ICardZone
{
    string GetZoneName();
    void Remove(Card card);
    void Add(Card card, CardZoneChangeType type);
}
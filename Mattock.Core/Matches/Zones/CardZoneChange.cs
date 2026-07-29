using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Zones;

public class CardZoneChange(
    Card card,
    CardZoneChangeType type,
    ICardZoneChanger changer
)
{
    public Card Card { get; } = card;
    public CardZoneChangeType Type { get; private set; } = type;
    public ICardZoneChanger Changer { get; private set; } = changer;

    public string? Process()
    {
        if (!Changer.Accepts(Card))
        {
            return null;
        }
        Card.Zone?.Remove(Card);

        Card.SetZone(Changer.GetTargetZone());
        return Changer.Do(Card, Type);
    }
}

public enum CardZoneChangeType
{
    Bottom,
    Top,
}
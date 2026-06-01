using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class CardZoneChange
{
    public Card Card { get; }
    public ICardZone ToZone { get; private set; }
    public CardZoneChangeType Type { get; private set; }

    public CardZoneChange(
        Card card,
        ICardZone toZone,
        CardZoneChangeType type
    )
    {
        Card = card;
        ToZone = toZone;
        Type = type;
    }

    public void SetToZone(ICardZone toZone)
    {
        ToZone = toZone;
    }

    public void Process()
    {
        if (!ToZone.Accepts(Card))
        {
            return;
        }
        Card.Zone?.Remove(Card);

        Card.SetZone(ToZone);
        ToZone.Add(Card, Type);
    }
}

public enum CardZoneChangeType
{
    Bottom,
    Top,
}
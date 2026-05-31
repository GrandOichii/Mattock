using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class CardZoneChange
{
    public Card Card { get; }
    public ICardZone FromZone { get; }
    public ICardZone ToZone { get; private set; }
    public CardZoneChangeType Type { get; private set; }

    public CardZoneChange(
        Card card,
        ICardZone fromZone,
        ICardZone toZone,
        CardZoneChangeType type
    )
    {
        Card = card;
        FromZone = fromZone;
        ToZone = toZone;
        Type = type;
    }

    public void SetToZone(ICardZone toZone)
    {
        ToZone = toZone;
    }

    public void Process()
    {
        FromZone.Remove(Card);
        ToZone.Add(Card, Type);
    }
}

public enum CardZoneChangeType
{
    Bottom,
    Top,
}
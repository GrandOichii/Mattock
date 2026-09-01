using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;

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

    public async Task<CardZoneChangeResult> Process()
    {
        if (!Changer.Accepts(Card))
        {
            return new(null, null);
        }
        
        Card.Zone?.Remove(Card);

        Card.SetZone(Changer.GetTargetZone());
        return await Changer.Do(Card, Type);
    }
}

public enum CardZoneChangeType
{
    Bottom,
    Top,
}
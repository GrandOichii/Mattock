using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Zones;

public interface ICardZone
{
    string GetZoneName();

    void Remove(Card card);

    (Card, Player)[] GetCardControllerPairs();
}

public interface ICardZoneChanger
{
    Task<CardZoneChangeResult> Do(Card card, CardZoneChangeType type);

    public async Task<CardZoneChangeResult> Move(Card card, CardZoneChangeType type)
    {
        var result = await Do(card, type);
        card.UpdateTimestamp();
        return result;
    }

    bool Accepts(Card card);
    
    ICardZone GetTargetZone();
}

public record CardZoneChangeResult(
    string? Id,
    RollbackRequest? Request
);
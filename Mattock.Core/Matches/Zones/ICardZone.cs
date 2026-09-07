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

    bool Accepts(Card card);
    
    ICardZone GetTargetZone();
}

public record CardZoneChangeResult(
    string? Id,
    RollbackRequest? Request
);
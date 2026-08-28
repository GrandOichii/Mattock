using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Players.Cards.CardZones;

public abstract class OwnedCardZone(
    Player player
) : ICardZone
{
    public abstract string GetZoneName();

    public Match Match { get; } = player.Match;
    public Player Player { get; } = player;
    public List<Card> Cards { get; private set; } = [];

    public int GetCount() => Cards.Count;

    public void Shuffle()
    {
        // Owner.Match.Logger?.LogDebug("Shuffling MatchCardCollection {ZoneLogName}", ZoneLogName);
        Cards = [.. Cards.OrderBy(_ => Match.Rng.Next())];
    }

    public void AddRaw(Card card)
    {
        Cards.Add(card);
        card.SetZone(this);
    }

    public Card? GetLast()
    {
        return Cards.LastOrDefault();
    }

    public void Remove(Card card)
    {
        if (Cards.Remove(card)) return;

        // TODO this may need to be removed
        throw new CodeErrorException($"Failed to remove card {card.GetDisplayName()} from zone \"{GetZoneName()}\" of player {Player.GetDisplayName()}");
    }

    public ICardZoneChanger GetCardZoneChanger()
        => new CardZoneChanger(this);

    class CardZoneChanger(
        OwnedCardZone zone
    ) : ICardZoneChanger
    {
        public bool Accepts(Card card)
        {
            // TODO
            return true;
        }

        public string Do(Card card, CardZoneChangeType type)
        {
            var match = zone.Player.Match;

            // * 400.3. If an object would go to any library, graveyard, or hand other than its owner’s, it goes to its owner’s corresponding zone.
            if (card.OwnerIdx != zone.Player.Idx)
            {
                var newZone = match.Players[card.OwnerIdx].GetZoneByName(zone.GetZoneName());
                return new CardZoneChanger(newZone).Do(card, type);
            }
            
            switch (type)
            {
                case CardZoneChangeType.Bottom:
                    zone.Cards.Add(card);
                    return card.Id;
                case CardZoneChangeType.Top:
                    zone.Cards.Insert(0, card);
                    return card.Id;
                default:
                    throw new CodeErrorException($"Unrecognized {nameof(CardZoneChangeType)}: {type}");
            };
        }

        public ICardZone GetTargetZone()
            => zone;
    }
}
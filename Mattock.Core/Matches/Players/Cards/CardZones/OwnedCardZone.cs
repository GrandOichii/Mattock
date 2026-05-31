namespace Mattock.Core.Matches.Players.Cards.CardZones;

public abstract class OwnedCardZone : ICardZone
{
    public abstract string GetZoneName();

    public Match Match { get; }
    public Player Player { get; }
    public List<Card> Cards { get; private set; }

    public OwnedCardZone(Player player)
    {
        Match = player.Match;
        Player = player;
        Cards = [];
    }

    public int GetCount() => Cards.Count;

    public void Shuffle()
    {
        // Owner.Match.Logger?.LogDebug("Shuffling MatchCardCollection {ZoneLogName}", ZoneLogName);
        Cards = [.. Cards.OrderBy(_ => Match.Rng.Next())];
    }

    public void AddRaw(Card card)
    {
        Cards.Add(card);
    }

    public Card? GetLast()
    {
        return Cards.LastOrDefault();
    }

    public void Remove(Card card)
    {
        if (Cards.Remove(card)) return;

        // TODO this may need to be removed
        throw new Exception($"Failed to remove card {card.GetDisplayName()} from zone \"{GetZoneName()}\" of player {Player.GetDisplayName()}");
    }

    public void Add(Card card, CardZoneChangeType type)
    {
        switch (type)
        {
            case CardZoneChangeType.Bottom:
                Cards.Insert(0, card);
                return;
            case CardZoneChangeType.Top:
                Cards.Add(card);
                return;
        };
    }
}
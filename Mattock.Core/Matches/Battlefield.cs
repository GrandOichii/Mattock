using Mattock.Core.Matches.Objects;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class Battlefield(Match match) : ICardZone
{
    public List<Permanent> Permanents { get; } = [];

    public void Add(Card card, CardZoneChangeType type)
    {
        // * type doesn't matter
        var permanent = new Permanent(card);
        Permanents.Add(permanent);
    }

    public string GetZoneName() => "Battlefield";

    public void Remove(Card card)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Permanent? GetPermanentById(string id) => Permanents.SingleOrDefault(p => p.Card.Id == id);

    public async Task MoveCard(Card card, Player controller)
    {
        match.MoveCard(
            card,
            this,
            CardZoneChangeType.Bottom
        );

        var permanent = GetPermanentById(card.Id);
        if (permanent is null) return;

        permanent.SetController(controller);
    }

    public bool Accepts(Card card)
    {
        return !card.IsSorcery() && !card.IsInstant();
    }
}
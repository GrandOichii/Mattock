using Mattock.Core.Matches.Objects;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class Battlefield(Match match) : ICardZone
{
    private readonly List<Permanent> _permanents = [];

    private int _lastPid = 0;

    public string GeneratePid() => $"p{++_lastPid}";

    public string Add(Card card, CardZoneChangeType type)
    {
        // * type doesn't matter
        var permanent = new Permanent(card);
        _permanents.Add(permanent);
        return permanent.Pid;
    }

    public string GetZoneName() => "Battlefield";

    public void Remove(Card card)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Permanent? GetPermanentById(string id) => _permanents.SingleOrDefault(p => p.Card.Id == id);
    public Permanent? GetPermanentByPid(string pid) => _permanents.SingleOrDefault(p => p.Pid == pid);

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

    public Permanent[] GetPermanents()
    {
        // TODO check if any need to be put in graveyard
        return [ .. _permanents ];
    }
}
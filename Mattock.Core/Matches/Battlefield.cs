using Mattock.Core.Matches.Objects;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches;

public class Battlefield(Match match) : ICardZone
{
    public void Add(Card card, CardZoneChangeType type)
    {
        throw new NotImplementedException();
    }

    public string GetZoneName() => "Battlefield";

    public void Remove(Card card)
    {
        throw new NotImplementedException();
    }
}
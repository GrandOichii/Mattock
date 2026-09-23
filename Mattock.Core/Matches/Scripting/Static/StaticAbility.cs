using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Scripting.Static;

public class StaticAbility(
    Match match,
    StaticAbilityTemplate sat,
    Card card
)
{
    public string StaticAbilityId { get; } = match.Ids.GenerateActivatedAbilityId();
    public Card Card { get; } = card;
    public StaticAbilityTemplate Template { get; } = sat;

    public bool ActiveInZone(ICardZone zone)
    {
        throw new NotImplementedException();
    }
}
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Scripting;

public class Ability(
    Match match,
    Card card,
    Effect[] effects
)
{
    public Card Card { get; } = card;
    public Effect[] Effects { get; } = effects;
    public Match Match { get; } = match;

    
    public Target[] GetTargets()
    {
        return [.. Effects.SelectMany(e => e.Targets)];
    }
}
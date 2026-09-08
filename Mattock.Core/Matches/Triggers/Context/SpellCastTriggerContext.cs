using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context.Data;

namespace Mattock.Core.Matches.Triggers.Context;

public class SpellCastTriggerContext(
    Card card,
    Player caster
) : ITriggerContext
{
    public Card Card { get; } = card;
    public Player Caster { get; } = caster;
}
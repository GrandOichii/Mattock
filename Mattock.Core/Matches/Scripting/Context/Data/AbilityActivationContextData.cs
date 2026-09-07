using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Scripting.Context.Data;

public class AbilityActivationContextData(
    // Player owner,
    ActivatedAbility ability
) : IEffectContextData
{
    public Card Source { get; } = ability.Card;
}
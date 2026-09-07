using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Triggered;

namespace Mattock.Core.Matches.Scripting.Context.Data;

public class AbilityTriggerContextData(
    // TriggeredAbility ability,
    Card source
) : IEffectContextData
{
    public Card Source { get; } = source;
}
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Triggered;

namespace Mattock.Core.Matches.Triggers;

public class QueuedTriggeredAbility(
    TriggeredAbility ability,
    EffectContext ctx
)
{
    public EffectContext Ctx { get; } = ctx;
    public TriggeredAbility Ability { get; } = ability;
}
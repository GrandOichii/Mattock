using Mattock.Core.Matches.Scripting.Triggered;
using Mattock.Core.Matches.Triggers.Context;

namespace Mattock.Core.Matches.Triggers;

public class Trigger(
    TriggerType type,
    ITriggerContext ctx
)
{
    public TriggerType Type { get; } = type;
    public ITriggerContext Ctx { get; } = ctx;
}
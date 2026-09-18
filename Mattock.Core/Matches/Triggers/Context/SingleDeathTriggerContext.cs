using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Triggers.Context;

public class SingleDeathTriggerContext(
    Permanent permanent
) : ITriggerContext
{
    public Permanent Permanent { get; } = permanent;
}
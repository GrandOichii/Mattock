using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Triggers.Context;

public class ETBTriggerContext(
    Permanent permanent,
    Player controller
) : ITriggerContext
{
    public Permanent Permanent { get; } = permanent;
    public Player Controller { get; } = controller;
}
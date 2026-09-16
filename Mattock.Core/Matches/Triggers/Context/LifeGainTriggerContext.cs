using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Triggers.Context;

public class LifeGainTriggerContext(
    Player player,
    int gained
) : ITriggerContext
{
    public Player Player { get; } = player;
    public int Gained { get; } = gained;
}
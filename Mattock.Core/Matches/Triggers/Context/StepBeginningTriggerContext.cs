using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Triggers.Context;

public class StepBeginningTriggerContext(
    StepType stepType,
    Player player
) : ITriggerContext
{
    public int StepType { get; } = (int)stepType;
    public Player Player { get; } = player;
}
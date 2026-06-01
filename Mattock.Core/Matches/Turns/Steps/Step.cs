using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps;

public abstract class Step
{
    public Match Match { get; }
    public Phase Phase { get; }
    public StepType Type { get; }
    public bool ActivePlayerReceivesPriority { get; }

    public Step(Phase phase, StepType type, bool activePlayerReceivesPriority)
    {
        Phase = phase;
        Match = phase.Match;
        Type = type;
        ActivePlayerReceivesPriority = activePlayerReceivesPriority;
    }

    public abstract bool CanBeTaken();
    public abstract Task DoPrePriority();
    public abstract Task DoPostPriority();
}
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps;

// TODO add 500.5.

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

    public async Task Do()
    {
        await DoPrePriority();
        if (ActivePlayerReceivesPriority)
        {
            // TODO? if the stack had resolved effects, does the active player still gain priority?

            await Match.CreateAndResolvePriority();
        }
        if (Match.AreWinnersDecided()) return;

        await DoPostPriority();

        // 500.5.
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();

        if (!Match.Stack.IsEmpty())
            throw new Exception($"Code error: the stack was not empty at the end of the step {Type}");
    }
}
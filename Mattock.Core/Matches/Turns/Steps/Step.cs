using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps;

// TODO add 500.5.

public abstract class Step(
    Phase phase,
    StepType type,
    bool activePlayerReceivesPriority
)
{
    public Match Match { get; } = phase.Match;
    public Phase Phase { get; } = phase;
    public StepType Type { get; } = type;
    public bool ActivePlayerReceivesPriority { get; } = activePlayerReceivesPriority;

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
        if (Match.ShouldHalt()) return;

        await DoPostPriority();

        // 500.5.
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();

        if (!Match.Stack.IsEmpty())
            throw new Exception($"Code error: the stack was not empty at the end of the step {Type}");
    }
}
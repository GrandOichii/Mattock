using Mattock.Core.Matches.Rollback;
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
    public abstract Task<RollbackRequest?> DoPrePriority();
    public abstract Task<RollbackRequest?> DoPostPriority();

    public async Task<RollbackRequest?> Do()
    {
        var request = await DoPrePriority();
        if (request is not null)
            return request;
        if (Match.ShouldHalt())
            return null;

        if (ActivePlayerReceivesPriority)
        {
            // TODO? if the stack had resolved effects, does the active player still gain priority?

            var (_, r) = await Match.CreateAndResolvePriority();
            if (r is not null)
                return r;
            if (Match.ShouldHalt())
                return null;
        }

        request = await DoPostPriority();
        if (request is not null)
            return request;
        if (Match.ShouldHalt())
            return null;

        // 500.5.
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();

        if (!Match.Stack.IsEmpty())
            throw new Exception($"Code error: the stack was not empty at the end of the step {Type}");
            
        return null;
    }
}
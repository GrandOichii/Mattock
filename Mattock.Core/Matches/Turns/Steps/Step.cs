using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps;

// TODO add 500.5.

public abstract class Step(
    Phase phase,
    StepType type,
    IStepPart[] parts
)
{
    public Match Match { get; } = phase.Match;
    public Phase Phase { get; } = phase;
    public StepType Type { get; } = type;
    public IStepPart[] Parts { get; } = parts;
    public int PartIdx { get; set; } = 0;

    public abstract bool CanBeTaken();

    public async Task<RollbackRequest?> Do()
    {
        for (; PartIdx < Parts.Length; ++PartIdx)
        {
            var request = await Parts[PartIdx].Do(Match);
            if (request is not null)
                return request;
            if (Match.ShouldHalt())
                return null;
        }

        // 500.5.
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();

        if (!Match.Stack.IsEmpty())
            throw new CodeErrorException($"The stack was not empty at the end of the step {Type}");
            
        return null;
    }
}
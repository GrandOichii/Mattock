using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Turns.Phases;

// TODO add 500.5.

public class Phase(
    Match match,
    PhaseType type,
    List<Step> steps
) : IHasSnapshot<Phase.Snapshot>
{
    public int CurrentStepIdx { get; private set; } = 0;
    public Match Match { get; } = match;
    public PhaseType Type { get; } = type;
    public List<Step> Steps { get; } = steps;

    public bool IsMainPhase() => Type == PhaseType.PrecombatMain || Type == PhaseType.PostcombatMain;

    public async Task Do()
    {
        await DoPreSteps();
        await DoSteps();
        if (Match.AreWinnersDecided()) return;
        await DoPostSteps();

        // 500.5.
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();
    }

    public virtual Task DoPreSteps()
    {
        return Task.CompletedTask;
    }

    public virtual Task DoPostSteps()
    {
        return Task.CompletedTask;
    }

    public async Task DoSteps()
    {
        for (; CurrentStepIdx < Steps.Count; ++CurrentStepIdx)
        {
            var step = Steps[CurrentStepIdx];

            if (!step.CanBeTaken()) continue;

            await step.Do();

            if (Match.AreWinnersDecided())
            {
                return;
            }
        }
    }

    public Step? GetCurrentStep() => CurrentStepIdx >= Steps.Count ? null : Steps[CurrentStepIdx];

    public Snapshot GetSnapshot()
    {
        return new()
        {
            CurrentStepIdx = CurrentStepIdx,
            Type = Type,
        };
    }

    public class Snapshot
    {
        public required int CurrentStepIdx { get; init; }
        public required PhaseType Type { get; init; }
    }
}
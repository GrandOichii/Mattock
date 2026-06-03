using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Turns.Phases;

// TODO add 500.5.

public class Phase
{
    public int CurrentStepIdx { get; private set; }
    public Match Match { get; }
    public PhaseType Type { get; }
    public List<Step> Steps { get; }

    public Phase(Match match, PhaseType type, List<Step> steps)
    {
        Match = match;
        Type = type;
        Steps = steps;
        CurrentStepIdx = 0;
    }

    public bool IsMainPhase() => Type == PhaseType.PrecombatMain || Type == PhaseType.PostcombatMain;

    public async Task Do()
    {
        await DoPreSteps();
        await DoSteps();
        await DoPostSteps();

        // 500.5.
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
        }
    }

    public Step? GetCurrentStep() => CurrentStepIdx >= Steps.Count ? null : Steps[CurrentStepIdx];
}
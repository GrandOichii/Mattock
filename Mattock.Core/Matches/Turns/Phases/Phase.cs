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

    public virtual Task DoPrePhases()
    {
        return Task.CompletedTask;
    }

    public virtual Task DoPostPhases()
    {
        return Task.CompletedTask;
    }

    public async Task DoPhases()
    {
        for (; CurrentStepIdx < Steps.Count; ++CurrentStepIdx)
        {
            var step = Steps[CurrentStepIdx];

            if (!step.CanBeTaken()) continue;

            await step.DoPrePriority();

            // await Match.Stack.Resolve();

            if (step.ActivePlayerReceivesPriority)
            {
                // TODO? if the stack had resolved effects, does the active player still gain priority?

                await Match.CreateAndResolvePriority();
            }

            await step.DoPostPriority();

            if (!Match.Stack.IsEmpty())
                throw new Exception($"Code error: the stack was not empty at the end of the step {step.Type}");
        }
    }

    public Step? GetCurrentStep() => CurrentStepIdx >= Steps.Count ? null : Steps[CurrentStepIdx];
}
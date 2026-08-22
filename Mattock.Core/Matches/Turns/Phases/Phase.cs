using System.Security.Cryptography.X509Certificates;
using Mattock.Core.Matches.Rollback;
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

    public async Task<RollbackRequest?> Do()
    {
        RollbackRequest? rollback = await DoPreSteps();
        if (rollback is not null)
            return rollback;
        if (Match.ShouldHalt())
            return null;

        rollback = await DoSteps();
        if (rollback is not null)
            return rollback;
        if (Match.ShouldHalt())
            return null;

        rollback = await DoPostSteps();
        if (rollback is not null)
            return rollback;
        if (Match.ShouldHalt())
            return null;

        // 500.5.
        // TODO this might be an event
        if (Match.Config.ManaPoolEmptiesAtEndOfEachStep)
            Match.EmptyManaPools();

        return null;
    }

    public virtual Task<RollbackRequest?> DoPreSteps()
    {
        return Task.FromResult<RollbackRequest?>(null);
    }

    public virtual Task<RollbackRequest?> DoPostSteps()
    {
        return Task.FromResult<RollbackRequest?>(null);
    }

    public async Task<RollbackRequest?> DoSteps()
    {
        for (; CurrentStepIdx < Steps.Count; ++CurrentStepIdx)
        {
            var step = Steps[CurrentStepIdx];

            if (!step.CanBeTaken()) continue;

            var request = await step.Do();

            if (request is not null)
                return request;
            if (Match.ShouldHalt())
                return null;
        }
        return null;
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

    public void LoadSnapshot(Snapshot snapshot)
    {
        CurrentStepIdx = snapshot.CurrentStepIdx;
    }

    public class Snapshot
    {
        public required int CurrentStepIdx { get; init; }
        public required PhaseType Type { get; init; }
    }
}
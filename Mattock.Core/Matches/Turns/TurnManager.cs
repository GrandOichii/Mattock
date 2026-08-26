using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps.Beginning;
using Mattock.Core.Matches.Turns.Steps.Ending;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(
    Match _match
) : IHasSnapshot<TurnManager.Snapshot>
{
    public int ActivePlayerIdx { get; set; } = -1;
    public List<Phase> Phases { get; } = [];
    public int CurrentPhaseIdx { get; private set; } = 0;
    public int TurnCounter { get; set; } = 0;

    public int NextInTurnOrderIdx(int playerIdx)
    {
        int result = playerIdx;
        Player player;
        do
        {
            result = (result + 1) % _match.Players.Length;
            player = _match.Players[result];
        }
        while (!player.IsInGame());

        return result;
    }

    public Phase CreatePhase(PhaseType type)
    {
        return type switch
        {
            PhaseType.Beginning => new BeginningPhase(_match),
            PhaseType.PrecombatMain => new MainPhase(_match, true),
            PhaseType.Combat => new CombatPhase(_match),
            PhaseType.PostcombatMain => new MainPhase(_match, false),
            PhaseType.Ending => new EndingPhase(_match),
            _ => throw new Exception($"Unrecognized phase type: {type}") // TODO type
        };
    }

    public void ResetTurn()
    {
        Phases.Clear();
        CurrentPhaseIdx = 0;

        PhaseType[] phases = [
            PhaseType.Beginning,
            PhaseType.PrecombatMain,
            PhaseType.Combat,
            PhaseType.PostcombatMain,
            PhaseType.Ending,
        ];
        foreach (var type in phases)
            Phases.Add(CreatePhase(type));
    }

    public void AdvanceTurn()
    {
        if (_match.ShouldHalt()) return;

        // TODO implement extra turns

        ActivePlayerIdx = NextInTurnOrderIdx(ActivePlayerIdx);
    }

    public void AdvancePhase()
    {
        ++CurrentPhaseIdx;
    }

    public bool IsTurnEnded()
    {
        return CurrentPhaseIdx >= Phases.Count;
    }

    public async Task DoTurn()
    {
        ++TurnCounter;
        ResetTurn();
        _match.Snapshots.CreateSnapshot($"turn-{TurnCounter}");

        while (!IsTurnEnded())
        {
            var phase = GetCurrentPhase();

            var request = await phase.Do();
            if (request is not null)
            {
                var snap = _match.Snapshots.Get(request.RequestedSnapshotId)
                    ?? throw new Exception($"Requested to rollback to snapshot with unkown id: {request.RequestedSnapshotId}");
                _match.LoadSnapshot(snap.Snap);
                continue;
            }
            
            if (_match.ShouldHalt())
                return;

            AdvancePhase();
        }

        AdvanceTurn();

        foreach (var p in _match.Players)
            p.ResetTrackers();
    }

    public Phase GetCurrentPhase() => Phases[CurrentPhaseIdx];

    public Snapshot GetSnapshot()
    {
        var phase = GetCurrentPhase();
        int? stepIdx = phase.CurrentStepIdx < phase.Steps.Count 
            ? phase.Steps[phase.CurrentStepIdx].PartIdx 
            : null;

        return new(
            TurnCounter,
            ActivePlayerIdx,
            CurrentPhaseIdx,
            phase.CurrentStepIdx,
            stepIdx
        );
    }

    public record Snapshot
    (
        int TurnCounter,
        int ActivePlayerIdx,
        int CurrentPhaseIdx,
        int CurrentStepIdx,
        int? CurrentStepPartIdx
    );
}
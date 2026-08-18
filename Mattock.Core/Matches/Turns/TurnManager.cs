using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps.Beginning;
using Mattock.Core.Matches.Turns.Steps.Ending;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(
    Match match
) : IHasSnapshot<TurnManager.Snapshot>
{
    public int ActivePlayerIdx { get; set; } = -1;
    public List<Phase> Phases { get; } = [];
    public int CurrentPhaseIdx { get; private set; } = 0;

    public int NextInTurnOrderIdx(int playerIdx)
    {
        int result = playerIdx;
        Player player;
        do
        {
            result = (result + 1) % match.Players.Length;
            player = match.Players[result];
        }
        while (!player.IsInGame());

        return result;
    }

    public Phase CreatePhase(PhaseType type)
    {
        return type switch
        {
            PhaseType.Beginning => new BeginningPhase(match),
            PhaseType.PrecombatMain => new MainPhase(match, true),
            PhaseType.Combat => new CombatPhase(match),
            PhaseType.PostcombatMain => new MainPhase(match, false),
            PhaseType.Ending => new EndingPhase(match),
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
        if (match.ShouldHalt()) return;

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

    public Phase GetCurrentPhase() => Phases[CurrentPhaseIdx];

    public Snapshot GetSnapshot()
    {
        return new()
        {
            ActivePlayerIdx = ActivePlayerIdx,
            CurrentPhaseIdx = CurrentPhaseIdx,
            Phases = [.. Phases.Select(p => p.GetSnapshot())]
        };
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
        ActivePlayerIdx = snapshot.ActivePlayerIdx;
        CurrentPhaseIdx = snapshot.CurrentPhaseIdx;

        Phases.Clear();
        foreach (var phase in snapshot.Phases)
        {
            var p = CreatePhase(phase.Type);
            p.LoadSnapshot(phase);
        }
    }

    public class Snapshot
    {
        public required int ActivePlayerIdx { get; init; }
        public required int CurrentPhaseIdx { get; init; }
        public required List<Phase.Snapshot> Phases { get; init; }
    }
}
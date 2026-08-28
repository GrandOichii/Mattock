using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(
    Match _match
)
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
            _ => throw new CodeErrorException($"Unrecognized phase type: {type}"),
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

    public async Task<RollbackRequest?> DoTurn()
    {
        ++TurnCounter;
        ResetTurn();
        _match.Session.Snapshots.CreateSnapshot($"turn-{TurnCounter}");

        while (!IsTurnEnded())
        {
            var phase = GetCurrentPhase();

            var request = await phase.Do();
            if (request is not null)
            {
                return request;
            }
            
            if (_match.ShouldHalt())
                return null;

            AdvancePhase();
        }

        AdvanceTurn();

        foreach (var p in _match.Players)
            p.ResetTrackers();
            
        return null;
    }

    public Phase GetCurrentPhase() => Phases[CurrentPhaseIdx];
}
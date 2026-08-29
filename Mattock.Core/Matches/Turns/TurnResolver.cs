using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns;

public class TurnResolver
{
    private readonly Match _match;
    public List<Phase> Phases { get; } = [];
    public int CurrentPhaseIdx { get; private set; } = 0;

    public TurnResolver(Match match)
    {
        _match = match;

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

        _match.Session.Snapshots.CreateSnapshot($"Start of turn {match.TurnManager.TurnCounter}");
    }

    public async Task<RollbackRequest?> Resolve()
    {
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

        foreach (var p in _match.Players)
            p.ResetTrackers();
        
        return null;
    }

    public void AdvancePhase()
    {
        ++CurrentPhaseIdx;
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

    public bool IsTurnEnded()
    {
        return CurrentPhaseIdx >= Phases.Count;
    }

    public Phase GetCurrentPhase() => Phases[CurrentPhaseIdx];
}
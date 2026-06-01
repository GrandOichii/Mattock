using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps.Beginning;
using Mattock.Core.Matches.Turns.Steps.Ending;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(Match match)
{
    public List<Phase> Phases { get; } = [];
    public int CurrentPhaseIdx { get; private set; } = 0;

    public void Reset()
    {
        Phases.Clear();
        CurrentPhaseIdx = 0;

        var beginningPhase = new Phase(match, PhaseType.Beginning, []);
        beginningPhase.Steps.Add(new UntapStep(beginningPhase));
        beginningPhase.Steps.Add(new UpkeepStep(beginningPhase));
        beginningPhase.Steps.Add(new DrawStep(beginningPhase));

        Phases.Add(beginningPhase);
        Phases.Add(new MainPhase(match, true));
        Phases.Add(new CombatPhase(match));
        Phases.Add(new MainPhase(match, false));

        var endingPhase = new Phase(match, PhaseType.Ending, []);
        endingPhase.Steps.Add(new EndStep(endingPhase));
        endingPhase.Steps.Add(new CleanupStep(endingPhase));
        Phases.Add(endingPhase);
    }

    public void Advance()
    {
        ++CurrentPhaseIdx;
    }

    public bool IsTurnEnded()
    {
        return CurrentPhaseIdx >= Phases.Count;
    }

    public Phase GetCurrentPhase() => Phases[CurrentPhaseIdx];
}
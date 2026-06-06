using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps.Beginning;
using Mattock.Core.Matches.Turns.Steps.Ending;

namespace Mattock.Core.Matches.Turns;

public class TurnManager(Match match)
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

    public void ResetTurn()
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

    public void AdvanceTurn()
    {
        if (match.AreWinnersDecided()) return;
        // TODO
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
}
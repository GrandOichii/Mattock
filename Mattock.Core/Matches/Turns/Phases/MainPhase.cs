using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Turns.Phases;

public class MainPhase(
    Match match,
    bool precombat
) : Phase(match, precombat ? PhaseType.PrecombatMain : PhaseType.PostcombatMain, [])
{
    public async override Task DoPostSteps()
    {
        while (await Match.CreateAndResolvePriority());
    }
}
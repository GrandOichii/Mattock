using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Turns.Phases;

public class MainPhase : Phase
{
    public MainPhase(Match match, bool precombat) : base(
        match,
        precombat
            ? PhaseType.PrecombatMain
            : PhaseType.PostcombatMain,
        []
    )
    {
    }

    public async override Task DoPostSteps()
    {
        await Match.CreateAndResolvePriority();
    }
}
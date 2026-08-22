using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Matches.Turns.Phases;

public class MainPhase(
    Match match,
    bool precombat
) : Phase(match, precombat ? PhaseType.PrecombatMain : PhaseType.PostcombatMain, [])
{
    public async override Task<RollbackRequest?> DoPostSteps()
    {
        bool effectsResolved = true;
        while (effectsResolved && !Match.ShouldHalt())
        {
            RollbackRequest? rollback;
            (effectsResolved, rollback) = await Match.CreateAndResolvePriority();
            if (rollback is not null)
                return rollback;
        }
        return null;
    }
}
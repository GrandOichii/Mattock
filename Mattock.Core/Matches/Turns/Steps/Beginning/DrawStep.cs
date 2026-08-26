using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class DrawStep(
    Phase phase
): Step(
    phase,
    StepType.Draw,
    [
        new DrawStepPart(),
        new PriorityStepPart(),
    ]
)
{
    public override bool CanBeTaken()
    {
        return Match.TurnManager.TurnCounter > 1 ||
            !Match.Config.FirstPlayerNoDrawIfSingleOpponent || 
            Match.Players.Length > 2;
    }
}

public class DrawStepPart
    : IStepPart
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        return await match.Events.DrawCards([
            new(match.GetActivePlayer(), match.Config.DrawStepDrawAmount)
        ]);
    }
}
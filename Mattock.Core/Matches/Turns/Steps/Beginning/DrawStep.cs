using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class DrawStep(
    Phase phase
): Step(phase, StepType.Draw, true)
{
    public override bool CanBeTaken()
    {
        return Match.TurnCounter > 1 ||
            !Match.Config.FirstPlayerNoDrawIfSingleOpponent || 
            Match.Players.Length > 2;
    }

    public override async Task DoPrePriority()
    {
        await Match.Events.DrawCards([
            new(Match.GetActivePlayer(), Match.Config.DrawStepDrawAmount)
        ]);
    }

    public override Task DoPostPriority()
    {
        return Task.CompletedTask;
    }
}
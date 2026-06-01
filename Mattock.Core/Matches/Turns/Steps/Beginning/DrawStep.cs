using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Beginning;

public class DrawStep : Step
{
    public DrawStep(Phase phase) : base(
        phase,
        StepType.Draw,
        true
    )
    {
    }

    public override bool CanBeTaken()
    {
        return Match.TurnCounter > 1 ||
            !Match.Config.FirstPlayerNoDrawIfSingleOpponent || 
            Match.Players.Length > 2;
    }

    public override Task DoPrePriority()
    {
        Match.GetActivePlayer().Draw(Match.Config.DrawStepDrawAmount);
        return Task.CompletedTask;
    }

    public override Task DoPostPriority()
    {
        return Task.CompletedTask;
    }
}
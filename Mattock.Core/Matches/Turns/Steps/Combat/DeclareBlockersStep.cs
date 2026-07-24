using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareBlockersStep(
    Phase phase
) : Step(phase, StepType.DeclareBlockers, true)
{
    public override bool CanBeTaken()
    {
        return Match.Battlefield.GetAttackingPermanents().Length > 0;
    }

    public override Task DoPostPriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    public override async Task DoPrePriority()
    {
        // declare blockers
        var players = Match.GetPlayersInAPNAP();
        foreach (var player in players)
        {
            var available = player.GetAvailableBlockDeclarations();
            if (available.Length == 0) continue;

            // TODO
            var chosen = await player.ChooseBlockDeclarations(available);
            throw new NotImplementedException();           
        }
    }
}
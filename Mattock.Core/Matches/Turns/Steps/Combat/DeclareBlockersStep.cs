using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Rollback;
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

    public override Task<RollbackRequest?> DoPostPriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override async Task<RollbackRequest?> DoPrePriority()
    {
        // declare blockers
        var players = Match.GetPlayersInAPNAP();
        List<BlockDeclaration> declarations = [];
        foreach (var player in players)
        {
            var available = player.GetAvailableBlockDeclarations();
            if (available.Length == 0) continue;

            var (chosen, rollback) = await player.ChooseBlockDeclarations(available);
            if (rollback is not null)
                return rollback;

            declarations.AddRange(chosen);
        }
        
        var request = await Match.Events.DeclareBlockers([.. declarations]);
        if (request is not null)
            return request;
        return null;
    }
}
using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareBlockersStep(
    Phase phase
) : Step(
    phase,
    StepType.DeclareBlockers,
    [
        new DeclareBlockersStepPart(),
        new PriorityStepPart(),
    ]
)
{
    public override bool CanBeTaken()
    {
        return Match.Battlefield.GetAttackingPermanents().Length > 0;
    }
}

public class DeclareBlockersStepPart
    : IStepPart
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        // declare blockers
        var players = match.GetPlayersInAPNAP();
        List<BlockDeclaration> declarations = [];
        foreach (var player in players)
        {
            var available = player.GetAvailableBlockDeclarations();
            if (available.Length == 0) continue;

            var (chosen, rollbackRequest) = await player.ChooseBlockDeclarations(available);
            if (rollbackRequest is not null)
                return rollbackRequest;

            declarations.AddRange(chosen);
        }
        
        var rollback = await match.Events.DeclareBlockers([.. declarations]);
        if (rollback is not null)
            return rollback;
        return null;
    }
}
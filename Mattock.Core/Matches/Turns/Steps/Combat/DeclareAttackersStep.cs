
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareAttackersStep(
    Phase phase
) : Step(phase, StepType.DeclareAttackers, true)
{
    public override async Task<RollbackRequest?> DoPrePriority()
    {
        var active = Match.GetActivePlayer();

        // declare attacking creatures

        var available = active.GetAvailableAttackDeclarations();
        if (available.Length == 0)
            return null;
        
        var (declarations, rollback) = await active.ChooseAttackDeclarations(available);
        if (rollback is not null)
            return rollback;

        // check that there are no overlapping declarations
        List<Permanent> attackers = [];
        foreach (var d in declarations)
        {
            var conflict = declarations.FirstOrDefault(
                cd => d != cd && d.ConflictsWith(cd)
            );
            attackers.Add(d.Attacker);
            if (conflict is null) continue;

            throw new Exception($"Chosen attack declarations conflict with each other: {d.GetDisplayName()} and {conflict.GetDisplayName()}");
        }

        // TODO 508.1c

        // TODO 508.1d

        // TODO 508.1e

        // tap all attackers
        var request = await Match.Events.TapPermanents([.. attackers]);
        if (request is not null)
            return request;

        // TODO 508.1g

        // TODO 508.1h

        // TODO 508.1i

        // TODO 508.1j

        // turn creatures into attacking creatures
        request = await Match.Events.DeclareAttackers(declarations);
        if (request is not null)
            return request;

        // TODO 508.1m trigger
        return null;
    }
    
    public override Task<RollbackRequest?> DoPostPriority()
    {
        // TODO
        return Task.FromResult<RollbackRequest?>(null);
    }

    public override bool CanBeTaken() => true;
}

using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareAttackersStep(
    Phase phase
) : Step(phase, StepType.DeclareAttackers, true)
{
    public override async Task DoPrePriority()
    {
        var active = Match.GetActivePlayer();

        // declare attacking creatures

        var available = active.GetAvailableAttackDeclarations();
        if (available.Length == 0) return;
        
        var declarations = await active.ChooseAttackDeclarations(available);

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
        await Match.Events.TapPermanents([.. attackers]);

        // TODO 508.1g

        // TODO 508.1h

        // TODO 508.1i

        // TODO 508.1j

        // turn creatures into attacking creatures
        await Match.Events.DeclareAttackers(declarations);

        // TODO 508.1m trigger
    }

    
    public override Task DoPostPriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    public override bool CanBeTaken() => true;
}
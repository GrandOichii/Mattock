
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class DeclareAttackersStep : Step
{
    public DeclareAttackersStep(Phase phase) : base(phase, StepType.DeclareAttackers, true)
    {
    }

    public override async Task DoPrePriority()
    {
        var active = Match.GetActivePlayer();

        // declare attacking creatures

        var available = active.GetAvailableAttackDeclarations();
        var declarations = await active.ChooseAttackDeclarations(available);

        // check that there are no overlapping declarations
        foreach (var d in declarations)
        {
            var conflict = declarations.FirstOrDefault(
                cd => d != cd && d.ConflictsWith(d)
            );
            if (conflict is null) continue;

            throw new Exception($"Chosen attack declarations conflict with each other: {d.GetDisplayName()} and {conflict.GetDisplayName()}");
        }

        // TODO
    }

    
    public override Task DoPostPriority()
    {
        // TODO
        return Task.CompletedTask;
    }

    public override bool CanBeTaken() => true;
}
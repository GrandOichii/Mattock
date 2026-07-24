using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class CombatDamageStep(
    Phase phase
) : Step(phase, StepType.CombatDamage, true)
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

    public override Task DoPrePriority()
    {
        // TODO
        return Task.CompletedTask;
    }
}
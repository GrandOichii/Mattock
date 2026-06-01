namespace Mattock.Core.Matches.Turns.Steps;

public enum StepType
{
    Untap,
    Upkeep,
    Draw,
    BeginningOfCombat,
    DeclareAttackers, 
    DeclareBlockers, // Skipped if no creatures are declared as attackers or put on the battlefield attacking
    // FirstStrikeCombatDamage, // There are two combat damage steps if any attacking or blocking creature has first strike (see rule 702.7) or double strike (see rule 702.4).
    CombatDamage, // Skipped if no creatures are declared as attackers or put on the battlefield attacking
    EndOfCombat,
    End,
    Cleanup,
}
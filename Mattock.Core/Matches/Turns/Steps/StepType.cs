namespace Mattock.Core.Matches.Turns.Steps;

public enum StepType
{
    Untap = 0,
    Upkeep = 1,
    Draw = 2,
    BeginningOfCombat = 3,
    DeclareAttackers = 4, 
    DeclareBlockers = 5, // Skipped if no creatures are declared as attackers or put on the battlefield attacking
    // FirstStrikeCombatDamage = 6, // There are two combat damage steps if any attacking or blocking creature has first strike (see rule 702.7) or double strike (see rule 702.4).
    CombatDamage = 7, // Skipped if no creatures are declared as attackers or put on the battlefield attacking
    EndOfCombat = 8,
    End = 9,
    Cleanup = 10,
}
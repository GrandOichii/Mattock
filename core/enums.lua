CardTypes = {
    Artifact = 'Artifact',
    Creature = 'Creature',
    Enchantment = 'Enchantment',
    Instant = 'Instant',
    Land = 'Land',
    Planeswalker = 'Planeswalker',
    Sorcery = 'Sorcery',
    Kindred = 'Kindred',
    Dungeon = 'Dungeon',
    Battle = 'Battle',
    Phenomenon = 'Phenomenon',
    Vanguard = 'Vanguard',
    Conspiracy = 'Conspiracy',
}

ManaTypes = {
    White = 0,
    Blue = 1,
    Black = 2,
    Red = 3,
    Green = 4,
    Colorless = 5,
}

Colors = {
    White = 0,
    Blue = 1,
    Black = 2,
    Red = 3,
    Green = 4,
}

StepTypes = {
    Untap = 0,
    Upkeep = 1,
    Draw = 2,
    BeginningOfCombat = 3,
    DeclareAttackers = 4,
    DeclareBlockers = 5,
    -- FirstStrikeCombatDamage = 6,
    CombatDamage = 7,
    EndOfCombat = 8,
    End = 9,
    Cleanup = 10,
}

ZoneNames = {
    -- Shared zones
    Battlefield = 'Battlefield',
    Exile = 'Exile',
    TheStack = 'TheStack',
    
    -- Owned zones
    Graveyard = 'Graveyard',
    Hand = 'Hand',
    Library = 'Library',
}

TriggerTypes = {
    StepBeginning = 0,
    ETB = 1,
    SpellCast = 2,
}
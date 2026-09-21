namespace Mattock.Core.Matches.Scripting.Continuous;

public enum ContinuousEffectLayer
{
    // Layer 1
    CopiableValues = 0,
    FaceDownModifications = 1,
    // Layer 2
    ControlChanging = 2,
    // Layer 3
    TextChanging = 3,
    // Layer 4
    TypeChanging = 4,
    // Layer 5
    ColorChanging = 5,
    // Layer 6
    AbilityAdding_KeywordCounters_AbilityRemoving_AbilityRestricting = 6,
    // Layer 7
    PowerToughnessDefinition = 7,
    PowerToughnessSetting = 8,
    PowerToughnessModification = 9,
    PowerToughnessSwitching = 10,
}
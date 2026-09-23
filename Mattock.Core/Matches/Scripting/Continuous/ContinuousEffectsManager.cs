using Mattock.Core.Matches.Scripting.Static;

namespace Mattock.Core.Matches.Scripting.Continuous;

public class ContinuousEffectsManager(
    Match match
)
{
    private long _lastTimestamp = 0;

    private readonly ContinuousEffectLayer[] LAYERS = [
        ContinuousEffectLayer.CopiableValues,
        ContinuousEffectLayer.FaceDownModifications,
        ContinuousEffectLayer.ControlChanging,
        ContinuousEffectLayer.TextChanging,
        ContinuousEffectLayer.TypeChanging,
        ContinuousEffectLayer.ColorChanging,
        ContinuousEffectLayer.AbilityAdding_KeywordCounters_AbilityRemoving_AbilityRestricting,
        ContinuousEffectLayer.PowerToughnessDefinition,
        ContinuousEffectLayer.PowerToughnessSetting,
        ContinuousEffectLayer.PowerToughnessModification,
        ContinuousEffectLayer.PowerToughnessSwitching,
    ];

    public long CreateTimestamp() => ++_lastTimestamp;

    // public void Remove(ContinuousEffect effect)
    // {
    //     throw new NotImplementedException();
    // }

    public void Apply()
    {
        // var cards = match.GetCards();
        // List<StaticAbility> staticAbilities = [];
        // foreach (var card in cards)
        // {
        //     var abilities = card.GetStaticAbilities();
        //     foreach (var ability in abilities)
        //     {
        //         if (!ability.ActiveInZone(card.Zone))
        //             continue;
        //         staticAbilities.Add(ability);
        //     }
        // }

        // HashSet<StaticAbility> usedAbilities = [];
        

        // foreach (var layer in layers)
        // {
            
        // }
    }
}
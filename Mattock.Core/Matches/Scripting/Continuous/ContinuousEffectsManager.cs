using Mattock.Core.Matches.Players.Cards;
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

    // TODO weird name
    public void UpdateEffectsFor(Card[] cards)
    {
        List<StaticAbility> newAbilities = [];
        foreach (var card in cards)
        {
            var all = card.GetStaticAbilities();
            var active = GetActiveAbilitiesFor(card);
            
            foreach (var ability in all)
            {
                var shouldApply = ability.ActiveInZone(card.Zone);
                var isActive = active.Contains(ability);

                if (shouldApply && !isActive)
                    newAbilities.Add(ability);

                if (!shouldApply && isActive)
                    RemoveActive(ability);
            }
        }

        // FIXME this doesn't follow rule 613.7m
        foreach (var ability in newAbilities)
        {
            ability.UpdateTimestamp();
        }
    }

    private StaticAbility[] GetActiveAbilitiesFor(Card card)
    {
        throw new NotImplementedException();
    } 

    private void AddActiveAbility(StaticAbility ability)
    {
        ability.UpdateTimestamp();
        throw new NotImplementedException();
    }

    private void RemoveActive(StaticAbility ability)
    {
        throw new NotImplementedException();
    }

    public long CreateTimestamp() => ++_lastTimestamp;

    // public void Remove(ContinuousEffect effect)
    // {
    //     throw new NotImplementedException();
    // }

    public void Apply()
    {
        throw new NotImplementedException();
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
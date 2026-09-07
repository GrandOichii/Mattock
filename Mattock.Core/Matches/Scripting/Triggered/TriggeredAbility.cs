using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Matches.Triggers;

namespace Mattock.Core.Matches.Scripting.Triggered;

public class TriggeredAbility(
    Match match,
    TriggeredAbilityTemplate tat,
    Card card
) : Ability(match, card, [.. tat.Effects])
{
    public string TriggeredAbilityId { get; } = match.Ids.GenerateTriggeredAbilityId();
    public string Text { get; } = tat.Text;
    // public DynamicManaCost[] ManaCosts { get; } = [.. tat.ManaCosts];
    // public Cost[] Costs { get; } = [.. tat.Costs];
    public TriggerMatcher[] TriggerMatchers { get; } = [.. tat.TriggerMatchers];
    
    public EffectContext? CanTrigger(Player by, Card card, Trigger trigger)
    {
        EffectContext ctx = new(
            by,
            new AbilityTriggerContextData(
                // this,
                card
            ),
            new([])
        );

        if (TriggerMatchers.All(m => !m.CanTrigger(ctx, trigger)))
            return null;

        // if (!GetCostCollection(ctx).CanBePayed(ctx))
        //     return null;

        return ctx;

    }

    // public CostCollection GetCostCollection(EffectContext ctx)
    // {
    //     ManaCostsCollection manaCost = new([.. ManaCosts.Select(c => c.ToManaCost(ctx))]);

    //     return new(
    //         "TODO", [
    //             manaCost,
    //             .. Costs
    //         ]
    //     );
    // }


    // public bool IsManaAbility()
    // {
    //     if (GetTargets().Length > 0) 
    //         return false;

    //     // TODO check that is not a loyalty ability

    //     // TODO check every effect. if any of them produce mana, return true
        
    //     return Effects.Any(e => e.CanProduceMana);
    // }

    

}
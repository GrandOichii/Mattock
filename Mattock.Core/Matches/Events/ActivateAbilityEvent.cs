using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Matches.Events;

public class ActivateAbilityEvent(
    Player player,
    ActivatedAbility aa
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        TargetDeclarationCollection targets = new([]);

        EffectContext ctx = new(
            player,
            new AbilityActivationContextData(
                aa
            ),
            targets
        );

        // 602.2.
        // TODO

        // 602.2a
        // TODO

        // If an activated ability is being activated from a hidden zone, the card that has that ability is revealed (see rule 701.20a). 
        // TODO

        // That ability is created on the stack as an object that’s not a card. It becomes the topmost object on the stack.
        match.Stack.Create(
            ctx,
            new ActivatedAbilityResolver(aa)
        );

        // It has the text of the ability that created it, and no other characteristics.
        // TODO

        // 602.2b
        // TODO

        // The remainder of the process for activating an ability is identical to the process for casting a spell listed in rules 601.2b–i.

        // 601.2c Choose targets
        var request = await match.Events.ChooseTargetsForActivatedAbility(aa, ctx);
        if (request is not null)
            return request;

        // var 
        // TODO
        
        // 601.2d Announce divisions
        // TODO

        // 601.2e Check if the spell can be legally cast (733)
        // TODO

        // 601.2f Determine the spell cost
        var costs = aa.GetCostCollection(ctx);

        // 601.2g Activate mana abilities to pay for costs
        // TODO

        // 601.2h Pay the cost
        var rollback = await costs.Pay(ctx);
        if (rollback is not null)
            return rollback;
        if (match.ShouldHalt())
            return null;
        

        // 601.2i Modify characteristics
        // TODO

        return null;
    }
}
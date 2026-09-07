using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Stack.Resolvers;
using Mattock.Core.Matches.Triggers;

namespace Mattock.Core.Matches.Events;

public class TriggerAbilityEvent(
    QueuedTriggeredAbility ability
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        // 603.3.
        match.Stack.Create(
            ability.Ctx,
            new TriggeredAbilityResolver(ability.Ability)
        );

        // 603.3c
        // TODO

        // 603.3d The remainder of the process for putting a triggered ability on the stack is identical to the process for casting a spell listed in rules 601.2c–d.

        // 601.2c Choose targets
        var rollback = await match.Events.ChooseTargetsForAbility(ability.Ability, ability.Ctx);
        if (rollback is not null)
            return rollback;

        // var 
        // TODO
        
        // 601.2d Announce divisions
        // TODO

        // 601.2e Check if the spell can be legally cast (733)
        // TODO

        // 601.2f Determine the spell cost
        // var costs = aa.GetCostCollection(ctx);

        // 601.2g Activate mana abilities to pay for costs
        // TODO

        // 601.2h Pay the cost
        // rollback = await costs.Pay(ctx);
        // if (rollback is not null)
        //     return rollback;
        // if (match.ShouldHalt())
        //     return null;
        

        // 601.2i Modify characteristics
        // TODO

        return null;
    }
}
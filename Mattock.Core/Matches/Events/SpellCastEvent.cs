using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Events;

public class SpellCastEvent(
    Player player,
    Card card
) : IEvent
{
    public async Task<RollbackRequest?> Do(Match match)
    {
        TargetDeclarationCollection targets = new([]);

        EffectContext ctx = new(
            player,
            new SpellEffectContextData(
                // player,
                // card
            ),
            targets
        );

        // 601.2a Move the card onto the stack
        var effect = match.Stack.Create(
            card,
            ctx
        );

        // 601.2b Modal spells
        // TODO

        // 601.2c Choose targets
        var rollback = await match.Events.ChooseTargetsForSpell(card, ctx);
        if (rollback is not null)
            return rollback;

        // var 
        // TODO
        
        // 601.2d Announce divisions
        // TODO

        // 601.2e Check if the spell can be legally cast (733)
        // TODO

        // 601.2f Determine the spell cost
        var costVariations = card.GetCostCollections(player);
        if (costVariations.Count != new HashSet<string>(costVariations.Select(c => c.Text)).Count)
        {
            throw new CodeErrorException($"Computed cost variations with duplicate texts (texts: {string.Join(", ", costVariations.Select(c => $"\"{c.Text}\""))})");
        }
        
        CostCollection choice;
        (choice, rollback) = await player.ChooseCostCollection([.. costVariations], $"Choose how to pay for {card.GetDisplayName()}");
        if (rollback is not null)
            return rollback;

        // 601.2g Activate mana abilities to pay for costs
        // TODO

        // 601.2h Pay the cost
        rollback = await choice.Pay(ctx);
        if (rollback is not null)
            return rollback;

        // 601.2i Modify characteristics
        // TODO

        return null;
    }
}
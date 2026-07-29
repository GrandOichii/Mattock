using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;

namespace Mattock.Core.Matches.Events;

public class SpellCastEvent(
    Player player,
    Card card
) : IEvent
{
    public async Task Do(Match match)
    {
        TargetDeclarationCollection targets = new([]);

        // EffectContext ctx = 

        // 601.2a Move the card onto the stack
        var effect = match.Stack.Create(
            card,
            player,
            targets
        );

        // 601.2b Modal spells
        // TODO

        // 601.2c Choose targets
        // TODO
        
        // 601.2d Announce divisions
        // TODO

        // 601.2e Check if the spell can be legally cast (733)
        // TODO

        // 601.2f Determine the spell cost
        var costVariations = card.GetCostCollections(player);
        if (costVariations.Count != new HashSet<string>(costVariations.Select(c => c.Text)).Count)
        {
            throw new Exception($"Computed cost variations with duplicate texts (texts: {string.Join(", ", costVariations.Select(c => $"\"{c.Text}\""))})");
        }
        var choice = await player.ChooseCostCollection([.. costVariations], $"Choose how to pay for {card.GetDisplayName()}");

        // 601.2g Activate mana abilities to pay for costs
        // TODO

        // 601.2h Pay the cost
        await player.PayCost(card, choice);
        // TODO

        // 601.2i Modify characteristics
        // TODO

        // Triggers
        // TODO
    }
}
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;

namespace Mattock.Core.Matches.Scripting.Activated;

public class ActivatedAbility(
    Match match,
    ActivatedAbilityTemplate aat,
    Card card
)
{
    public string Id { get; } = match.GenerateActivatedAbilityId();
    public string Text { get; } = aat.Text;
    public DynamicManaCost[] ManaCosts { get; } = [.. aat.ManaCosts];
    public Cost[] Costs { get; } = [.. aat.Costs];
    public Effect[] Effects { get; } = [.. aat.Effects];
    public Card Card { get; } = card;
    
    public bool CanBeActivated(Player by)
    {
        // TODO some effects change this
        if (Card.OwnerIdx != by.Idx)
            return false;

        // TODO some effects change this
        if (!by.Match.Battlefield.Contains(Card))
            return false;

        EffectContext ctx = new(
            by,
            new AbilityActivationContextData(
                
            ),
            new([])
        );

        ManaCostsCollection manaCost = new([.. ManaCosts.Select(c => c.ToManaCost(ctx))]);

        if (!manaCost.CanPay(ctx))
            return false;

        if (!Costs.All(c => c.CanPay(ctx)))
            return false;

        return true;
    }
}
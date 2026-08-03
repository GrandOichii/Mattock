using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;
using Microsoft.VisualBasic;

namespace Mattock.Core.Matches;

public class MatchEvents(
    Match _match
)
{
    public async Task TapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent e = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            true
        );

        await _match.ProcessEvent(e);
    }

    public async Task UntapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent e = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            false
        );

        await _match.ProcessEvent(e);
    }

    public async Task DeclareAttackers(AttackDeclaration[] declarations)
    {
        AttackDeclarationEvent e = new(
            declarations
        );

        await _match.ProcessEvent(e);
    }

    public async Task DeclareBlockers(BlockDeclaration[] declarations)
    {
        BlockDeclarationEvent e = new(
            declarations
        );

        await _match.ProcessEvent(e);
    }

    public async Task RemoveAllFromCombat()
    {
        RemoveFromCombatEvent e = new(
            
        );

        await _match.ProcessEvent(e);
    }

    public async Task ProcessDamage(Damage.Damage[] assignments)
    {
        ProcessDamageEvent e = new(
            assignments
        );

        await _match.ProcessEvent(e);
    }

    public async Task DrawCards(CardDraw[] draws)
    {
        CardDrawEvent e = new(
            draws
        );

        await _match.ProcessEvent(e);
    }

    public async Task GainLife(LifeGain[] gains)
    {
        LifeGainEvent e = new(
            gains
        );

        await _match.ProcessEvent(e);
    }

    public async Task LoseLife(LifeLoss[] losses)
    {
        LifeLossEvent e = new(
            losses
        );

        await _match.ProcessEvent(e);
    }

    public async Task CastSpell(Player player, Card card)
    {
        SpellCastEvent e = new(
            player,
            card
        );

        await _match.ProcessEvent(e);
    }

    public async Task ChooseTargetsForSpell(Card card, EffectContext ctx)
    {
        ChooseTargetsForSpellEvent e = new(
            card,
            ctx
        );

        await _match.ProcessEvent(e);
    }

    public async Task ChooseTargetsForActivatedAbility(ActivatedAbility aa, EffectContext ctx)
    {
        ChooseTargetsForActivatedAbilityEvent e = new(
            aa,
            ctx
        );

        await _match.ProcessEvent(e);
    }

    public async Task ActivateAbility(Player player, ActivatedAbility aa)
    {
        ActivateAbilityEvent e = new(
            player,
            aa
        );

        await _match.ProcessEvent(e);
    }
}
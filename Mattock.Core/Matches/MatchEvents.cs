using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;
using Microsoft.VisualBasic;

namespace Mattock.Core.Matches;

public class MatchEvents(
    Match _match
)
{
    public async Task<RollbackRequest?> TapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent e = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            true
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> UntapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent e = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            false
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> DeclareAttackers(AttackDeclaration[] declarations)
    {
        AttackDeclarationEvent e = new(
            declarations
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> DeclareBlockers(BlockDeclaration[] declarations)
    {
        BlockDeclarationEvent e = new(
            declarations
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> RemoveAllFromCombat()
    {
        RemoveFromCombatEvent e = new(
            
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> ProcessDamage(Damage.Damage[] assignments)
    {
        ProcessDamageEvent e = new(
            assignments
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> DrawCards(CardDraw[] draws)
    {
        CardDrawEvent e = new(
            draws
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> Mill(Mill[] mills)
    {
        MillEvent e = new(
            mills
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> GainLife(LifeGain[] gains)
    {
        LifeGainEvent e = new(
            gains
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> LoseLife(LifeLoss[] losses)
    {
        LifeLossEvent e = new(
            losses
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> CastSpell(Player player, Card card)
    {
        SpellCastEvent e = new(
            player,
            card
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> ChooseTargetsForSpell(Card card, EffectContext ctx)
    {
        ChooseTargetsForSpellEvent e = new(
            card,
            ctx
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> ChooseTargetsForActivatedAbility(ActivatedAbility aa, EffectContext ctx)
    {
        ChooseTargetsForActivatedAbilityEvent e = new(
            aa,
            ctx
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> ActivateAbility(Player player, ActivatedAbility aa)
    {
        IEvent e = aa.IsManaAbility()
            ? new ActivateManaAbilityEvent(player, aa)
            : new ActivateAbilityEvent(player, aa)
        ;
        
        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> AddMana(Player[] players, ManaAmount[] mana)
    {
        AddGenericManaEvent e = new(
            players,
            mana
        );

        return await _match.ProcessEvent(e);
    }

    public async Task<RollbackRequest?> PutOntoTheBattlefield((Card, Player)[] pairs)
    {
        PutOntoTheBattlefieldEvent e = new(
            pairs
        );

        return await _match.ProcessEvent(e);
    }
}
using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
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

    public async Task ProcessDamage(DamageAssignment[] assignments)
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


    public async Task CastSpell(Player player, Card card)
    {
        SpellCastEvent e = new(
            player,
            card
        );

        await _match.ProcessEvent(e);
    }

}
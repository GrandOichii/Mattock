using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
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

    public async Task RemoveAllFromCombat()
    {
        RemoveFromCombatEvent e = new(
            
        );

        await _match.ProcessEvent(e);
    }
}
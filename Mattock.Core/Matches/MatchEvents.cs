using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches;

public class MatchEvents(
    Match _match
)
{
    public async Task TapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent untapEvent = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            true
        );

        await _match.ProcessEvent(untapEvent);
    }

    public async Task UntapPermanents(Permanent[] permanents)
    {
        PermanentStatusChangeEvent untapEvent = new(
            [.. permanents],
            Permanents.Statuses.PermanentStatusType.Tapped,
            false
        );

        await _match.ProcessEvent(untapEvent);
    }

    public async Task DeclareAttackers(AttackDeclaration[] declarations)
    {
        AttackDeclarationEvent declarationEvent = new(
            declarations
        );

        await _match.ProcessEvent(declarationEvent);
    }

    public async Task RemoveAllFromCombat()
    {
        
    }
}
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

namespace Mattock.Core.Matches.Events;

public class AttackDeclarationEvent(
    AttackDeclaration[] _declarations
) : IEvent
{
    public Task Do(Match match)
    {
        foreach (var declaration in _declarations)
        {
            var attacker = declaration.Attacker;

            attacker.SetAttackTarget(declaration.Target);
        }
        
        return Task.CompletedTask;
    }
}
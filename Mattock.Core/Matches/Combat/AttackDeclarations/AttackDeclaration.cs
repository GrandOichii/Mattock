using Mattock.Core.Matches.Combat.AttackDeclarations.Targets;
using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Combat.AttackDeclarations;

public class AttackDeclaration
{
    public required Permanent Attacker { get; init; }
    public required IAttackDeclarationTarget Target { get; init; }

    public bool ConflictsWith(AttackDeclaration other)
    {
        if (Attacker == other.Attacker) return true;
        
        // TODO 
        return false;
    }

    public string GetDisplayName()
    {
        // TODO
        return $"{Attacker.GetDisplayName()} -> {Target.GetDisplayName()}";
    }
}
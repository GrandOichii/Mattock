using Mattock.Core.Matches.Combat.AttackDeclarations.Targets;
using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Combat.AttackDeclarations;

public class AttackDeclaration
{
    public required Permanent Attacker { get; init; }
    public required IAttackDeclarationTarget Target { get; init; }

    public bool ConflictsWith(AttackDeclaration other)
    {
        if (Attacker == other.Attacker) 
            return true;

        return Target.ConflictsWith(other.Target);
    }

    public string GetDisplayName()
    {
        return $"{Attacker.GetDisplayName()} -> {Target.GetDisplayName()}";
    }
}
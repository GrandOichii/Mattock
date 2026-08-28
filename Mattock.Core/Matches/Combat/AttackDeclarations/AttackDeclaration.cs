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

    // TODO naming
    public bool MatchesShort(Short shortAD)
        => Attacker.PermanentId == shortAD.AttackerPermanentId
        && Target.GetTargetId() == shortAD.TargetId;

    // TODO naming
    public Short GetShort()
        => new()
        {
            AttackerPermanentId = Attacker.PermanentId,
            TargetId = Target.GetTargetId(),
        };

    // TODO naming
    public class Short
    {
        public required string AttackerPermanentId { get; init; }
        public required string TargetId { get; init; }
    }
}
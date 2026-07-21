namespace Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

public interface IAttackDeclarationTarget
{
    string GetDisplayName();

    object GetTarget();

    bool ConflictsWith(IAttackDeclarationTarget other);
}
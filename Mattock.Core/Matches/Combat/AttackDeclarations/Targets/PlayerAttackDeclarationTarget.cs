using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

public class PlayerAttackDeclarationTarget : IAttackDeclarationTarget
{
    public required Player Target { get; init; }

    public object GetTarget() => Target;

    public string GetDisplayName()
    {
        return Target.GetDisplayName();
    }

    public bool ConflictsWith(IAttackDeclarationTarget other)
    {
        return false;
    }
}
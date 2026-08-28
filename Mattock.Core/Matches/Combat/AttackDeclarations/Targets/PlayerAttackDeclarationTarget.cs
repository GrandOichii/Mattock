using System.Security;
using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Damage.Targets;
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

    public bool BelongsTo(Player player) => Target == player;

    public IDamageTarget? GetDamageAssignmentTarget()
        => new PlayerDamageTarget(Target);

    public string GetTargetId()
        => Target.GetId();
}
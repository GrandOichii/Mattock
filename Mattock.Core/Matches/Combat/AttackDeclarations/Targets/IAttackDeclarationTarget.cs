using Mattock.Core.Matches.Damage.Targets;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

public interface IAttackDeclarationTarget
{
    string GetDisplayName();

    object GetTarget();

    bool ConflictsWith(IAttackDeclarationTarget other);

    bool BelongsTo(Player player);

    // ! for permanents, first check that the permanent is still on the battlefield
    IDamageTarget? GetDamageAssignmentTarget();

    string GetTargetId();
}
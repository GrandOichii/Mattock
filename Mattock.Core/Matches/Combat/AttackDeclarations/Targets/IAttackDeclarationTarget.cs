using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

public interface IAttackDeclarationTarget
{
    string GetDisplayName();

    object GetTarget();

    bool ConflictsWith(IAttackDeclarationTarget other);

    bool BelongsTo(Player player);
}
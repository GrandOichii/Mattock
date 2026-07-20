using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

public class PlayerAttackDeclarationTarget : IAttackDeclarationTarget
{
    public required Player Target { get; init; }
}
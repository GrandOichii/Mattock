using Mattock.Core.Matches.Combat.AttackDeclarations.Targets;

namespace Mattock.Core.Matches.Permanents;

public class CombatState(
    Permanent permanent,
    IAttackDeclarationTarget attackTarget
)
{
    public Permanent Permanent { get; } = permanent;
    public IAttackDeclarationTarget AttackTarget { get; } = attackTarget;
    public List<Permanent> BlockedBy { get; } = [];
    public bool IsBlocked { get; private set; } = false;

    public void AddBlocker(Permanent permanent)
    {
        BlockedBy.Add(permanent);
        IsBlocked = true;
    }
    
}

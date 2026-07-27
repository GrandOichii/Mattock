using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Damage;

public class PermanentDamageAssignmentTarget(
    Permanent permanent
) : IDamageAssignmentTarget
{
    public void ProcessDamage(int damage)
    {
        permanent.DealDamage(damage);
    }
}
using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Damage.Targets;

public class PermanentDamageTarget(
    Permanent permanent
) : IDamageTarget
{
    public void ProcessDamage(int damage)
    {
        permanent.DealDamage(damage);
    }
}
using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Damage.Sources;

public class CombatDamageSource(
    Permanent permanent
) : IDamageSource
{
    public int GetDamage()
    {
        return permanent.GetPower();
    }
}
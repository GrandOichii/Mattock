namespace Mattock.Core.Matches.Damage.Sources;

public class TODODamageSource(
    int _damage
) : IDamageSource
{
    public int GetDamage()
    {
        return _damage;
    }
}
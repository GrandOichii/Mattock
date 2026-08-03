using Mattock.Core.Matches.Damage.Sources;
using Mattock.Core.Matches.Damage.Targets;

namespace Mattock.Core.Matches.Damage;

public class Damage(
    IDamageSource source,
    IDamageTarget target
)
{
    public IDamageSource Source { get; } = source;
    public IDamageTarget Target { get; } = target;

    public void Do()
    {
        var damage = Source.GetDamage();

        // TODO modifications
        Target.ProcessDamage(damage);
    }
}
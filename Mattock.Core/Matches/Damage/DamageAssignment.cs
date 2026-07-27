using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Damage;

public class DamageAssignment(
    Permanent source,
    IDamageAssignmentTarget target
)
{
    public Permanent Source { get; } = source;
    public IDamageAssignmentTarget Target { get; } = target;

    public void Do()
    {
        var damage = Source.GetPower();
        // TODO modifications
        Target.ProcessDamage(damage);
    }
}
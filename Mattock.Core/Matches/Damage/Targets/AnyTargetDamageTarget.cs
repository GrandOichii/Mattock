using Mattock.Core.Matches.Players.Controllers;

namespace Mattock.Core.Matches.Damage.Targets;

public class AnyTargetDamageTarget(
    IAnyTargetChoice anyTarget
) : IDamageTarget
{
    public void ProcessDamage(int damage)
    {
        anyTarget.ProcessDamage(damage);
    }
}
using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Damage.Targets;

public class PlayerDamageTarget(
    Player player
) : IDamageTarget
{
    public void ProcessDamage(int damage)
    {
        player.Life.DealDamage(damage);
    }
}
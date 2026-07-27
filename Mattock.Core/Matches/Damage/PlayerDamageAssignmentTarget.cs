using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Damage;

public class PlayerDamageAssignmentTarget(
    Player player
) : IDamageAssignmentTarget
{
    public void ProcessDamage(int damage)
    {
        player.Life.DealDamage(damage);
    }
}
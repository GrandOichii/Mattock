using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Matches.Players.Controllers;

public interface IAnyTargetChoice
{
    string ToUniqueString();
    void ProcessDamage(int damage);
}

public class PlayerAnyTargetChoice(
    Player player
) : IAnyTargetChoice
{
    public string ToUniqueString()
        => player.GetDisplayName();

    public void ProcessDamage(int damage)
    {
        player.Life.DealDamage(damage);
    }
}

public class PermanentAnyTargetChoice(
    Permanent permanent
) : IAnyTargetChoice
{
    public string ToUniqueString()
        => permanent.PermanentId;

    public void ProcessDamage(int damage)
    {
        permanent.DealDamage(damage);
    }
}


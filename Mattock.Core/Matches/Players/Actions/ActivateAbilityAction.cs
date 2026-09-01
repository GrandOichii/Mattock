using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Players.Actions;

public class ActivateAbilityAction : IAction
{
    public static readonly string ActionWord = "ActivateNonMana";

    public List<ICommand> GetAvailable(Player player)
    {
        var available = player.GetActivatableAbilities();

        return [.. available.Select(a => new ActivateAbilityCommand(player, a))];
    }
}

public class ActivateAbilityCommand(
    Player player,
    ActivatedAbility aa
) : ICommand
{
    public async Task<RollbackRequest?> Do()
    {
        var rollback = await player.Activate(aa);
        if (rollback is not null)
            return rollback;
            
        player.Match.ResetPriority(player.Idx);
        return null;
    }

    public string ToCommandString()
        => $"{ActivateAbilityAction.ActionWord} {aa.ActivatedAbilityId}";
}
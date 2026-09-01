using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Players.Actions;

public class ActivateManaAbilityAction : IAction
{
    public static readonly string ActionWord = "ActivateMana";

    public List<ICommand> GetAvailable(Player player)
    {
        var available = player.GetActivatableManaAbilities();

        return [.. available.Select(a => new ActivateManaAbilityCommand(player, a))];
    }
}

public class ActivateManaAbilityCommand(
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
        => $"{ActivateManaAbilityAction.ActionWord} {aa.ActivatedAbilityId}";
}
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Players.Actions;

public class ActivateActivatedAbilityAction : IAction
{
    public static readonly string ActionWord = "Activate";

    public List<ICommand> GetAvailable(Player player)
    {
        var available = player.GetActivatableAbilities();

        return [.. available.Select(a => new ActivateActivatedAbilityCommand(player, a))];
    }
}

public class ActivateActivatedAbilityCommand(
    Player player,
    ActivatedAbility aa
) : ICommand
{
    public async Task Do()
    {
        // await player.Cast(card);
        // player.Match.ResetPriority(player.Idx);
    }

    public string ToCommandString()
        => $"{ActivateActivatedAbilityAction.ActionWord} {aa.Id}";
}
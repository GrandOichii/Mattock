

namespace Mattock.Core.Matches.Players.Actions;

public class PassAction : IAction
{
    public static readonly string ActionWord = "Pass";

    public List<ICommand> GetAvailable(Player player)
    {
        return [
            new PassCommand(player)
        ];
    }
}

public class PassCommand(Player player) : ICommand
{
    public string ToCommandString() => PassAction.ActionWord;

    public Task Do()
    {
        var p = player.Match.Priority!;
        if (p.PriorityPlayerIdx != player.Idx)
            throw new Exception($"Code error: received pass command from non-priority player {player.GetDisplayName()} (priority player idx: {p.PriorityPlayerIdx})");

        p.Advance();
        return Task.CompletedTask;
    }
}
namespace Mattock.Core.Matches.Players.Actions;

public interface IAction
{
    // Task Do(T command);
    List<ICommand> GetAvailable(Player player);
}

public interface ICommand
{
    string ToCommandString();
    Task Do();
}
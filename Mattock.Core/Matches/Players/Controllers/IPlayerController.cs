namespace Mattock.Core.Matches.Players.Controllers;

public interface IPlayerController
{
    Task<Player> ChoosePlayer(Player player, Player[] options, string hint);
}
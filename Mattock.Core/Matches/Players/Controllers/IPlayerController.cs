using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Players.Controllers;

public interface IPlayerController
{
    Task Update(Player player, string? stateMsg = null);
    Task<ICommand> ChooseCommand(Player player, ICommand[] options);
    Task<Player> ChoosePlayer(Player player, Player[] options, string hint);
    Task<string> ChooseString(Player player, string[] options, string hint);
    Task<Card> ChooseCard(Player player, Card[] options, string hint);
}
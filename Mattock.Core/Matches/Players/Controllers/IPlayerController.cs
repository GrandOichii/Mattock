using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Players.Controllers;

public interface IPlayerController
{
    Task Update(Player player, string? stateMsg = null);
    Task<ICommand> ChooseCommand(Player player, ICommand[] options);
    Task<Player> ChoosePlayer(Player player, Player[] options, string hint);
    Task<string> ChooseString(Player player, string[] options, string hint);
    Task<Card> ChooseCard(Player player, Card[] options, string hint);
    Task<CostCollection> ChooseCostCollection(Player player, CostCollection[] options, string hint);
    Task<StoredMana> ChooseStoredMana(Player player, StoredMana[] options, string hint);
}
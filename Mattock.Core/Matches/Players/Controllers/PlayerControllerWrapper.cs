using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Players.Controllers;


public abstract class PlayerControllerWrapper(
    IPlayerController controller
) : IPlayerController
{
    public abstract Task HandleCommandChoice(ICommand choice, Player player, ICommand[] choices);
    public abstract Task HandlePlayerChoice(Player choice, Player player, Player[] choices, string hint);
    public abstract Task HandleStringChoice(string choice, Player player, string[] choices, string hint);
    public abstract Task HandleCardChoice(Card choice, Player player, Card[] choices, string hint);
    public abstract Task HandleCostCollectionChoice(CostCollection choice, Player player, CostCollection[] choices, string hint);
    public abstract Task HandleStoredManaChoice(StoredMana choice, Player player, StoredMana[] choices, string hint);

    // public void AddEvent(Event e)
    // {
    //     controller.AddEvent(e);
    // }

    // public void AddLog(Log l)
    // {
    //     HandleNewLog(l);
    //     controller.AddLog(l);
    // }

    public async Task Update(Player player)
    {
        await HandleUpdate(player);
        await controller.Update(player);
    }

    public virtual Task HandleUpdate(Player player)
    {
        return Task.CompletedTask;
    }

    public async Task<ICommand> ChooseCommand(Player player, Actions.ICommand[] options)
    {
        var result = await controller.ChooseCommand(player, options);
        await HandleCommandChoice(result, player, options);

        return result;
    }

    public async Task<Card> ChooseCard(Player player, Card[] options, string hint)
    {
        var result = await controller.ChooseCard(player, options, hint);
        await HandleCardChoice(result, player, options, hint);

        return result;
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        var result = await controller.ChoosePlayer(player, options, hint);
        await HandlePlayerChoice(result, player, options, hint);

        return result;
    }

    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        var result = await controller.ChooseString(player, options, hint);
        await HandleStringChoice(result, player, options, hint);

        return result;
    }
    

    public async Task<CostCollection> ChooseCostCollection(Player player, CostCollection[] options, string hint)
    {
        var result = await controller.ChooseCostCollection(player, options, hint);
        await HandleCostCollectionChoice(result, player, options, hint);

        return result;
    }
    
    public async Task<StoredMana> ChooseStoredMana(Player player, StoredMana[] options, string hint)
    {
        var result = await controller.ChooseStoredMana(player, options, hint);
        await HandleStoredManaChoice(result, player, options, hint);

        return result;
    }

    public Task Update(Player player, string? stateMsg = null)
    {
        return controller.Update(player, stateMsg);
    }

}
using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
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
    public abstract Task HandlePlayersChoice(Player[] choices, Player player, Player[] options, int min, int max, string hint);
    public abstract Task HandlePermanentsChoice(Permanent[] choices, Player player, Permanent[] options, int min, int max, string hint);
    public abstract Task HandleStringChoice(string? choice, Player player, string[] options, string hint);
    public abstract Task HandleCardChoice(Card? choice, Player player, Card[] options, string hint);
    public abstract Task HandleCostCollectionChoice(CostCollection? choice, Player player, CostCollection[] options, string hint);
    public abstract Task HandleStoredManaChoice(StoredMana? choice, Player player, StoredMana[] options, string hint);

    // public void AddEvent(Event , allowNonee)
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

    public async Task<ICommand> ChooseCommand(Player player, ICommand[] options)
    {
        var result = await controller.ChooseCommand(player, options);
        await HandleCommandChoice(result, player, options);

        return result;
    }

    public async Task<Card?> ChooseCard(Player player, Card[] options, string hint, bool allowNone)
    {
        var result = await controller.ChooseCard(player, options, hint, allowNone);
        await HandleCardChoice(result, player, options, hint);

        return result;
    }

    public async Task<Player[]> ChoosePlayers(
        Player player,
        Player[] options,
        int min,
        int max,
        string hint
    )
    {
        var result = await controller.ChoosePlayers(player, options, min, max, hint);
        await HandlePlayersChoice(result, player, options, min, max, hint);

        return result;
    }

    public async Task<Permanent[]> ChoosePermanents(
        Player player,
        Permanent[] options,
        int min,
        int max,
        string hint
    )
    {
        var result = await controller.ChoosePermanents(player, options, min, max, hint);
        await HandlePermanentsChoice(result, player, options, min, max, hint);

        return result;
    }

    public async Task<string?> ChooseString(Player player, string[] options, string hint, bool allowNone)
    {
        var result = await controller.ChooseString(player, options, hint, allowNone);
        await HandleStringChoice(result, player, options, hint);

        return result;
    }
    

    public async Task<CostCollection?> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
    {
        var result = await controller.ChooseCostCollection(player, options, hint, allowNone);
        await HandleCostCollectionChoice(result, player, options, hint);

        return result;
    }
    
    public async Task<StoredMana?> ChooseStoredMana(Player player, StoredMana[] options, string hint, bool allowNone)
    {
        var result = await controller.ChooseStoredMana(player, options, hint, allowNone);
        await HandleStoredManaChoice(result, player, options, hint);

        return result;
    }

    public Task Update(Player player, string? stateMsg = null)
    {
        return controller.Update(player, stateMsg);
    }

    public Task<AttackDeclaration[]> ChooseAttackDeclarations(Player player, AttackDeclaration[] options)
    {
        var result = controller.ChooseAttackDeclarations(player, options);
        // TODO handle

        return result;
    }

    public Task<BlockDeclaration[]> ChooseBlockDeclarations(Player player, BlockDeclaration[] options)
    {
        var result = controller.ChooseBlockDeclarations(player, options);
        // TODO handle

        return result;
    }
}
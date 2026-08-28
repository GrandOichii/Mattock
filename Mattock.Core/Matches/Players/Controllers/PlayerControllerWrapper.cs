using System.Security;
using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Players.Controllers;


public abstract class PlayerControllerWrapper(
    IPlayerController controller
) : IPlayerController
{
    private IPlayerController _controller = controller;

    public abstract Task HandleCommandChoice(
        ICommand choice,
        Player player,
        ICommand[] options
    );

    public abstract Task HandlePlayersChoice(
        Player[] choices,
        Player player,
        Player[] options,
        int min,
        int max,
        string hint
    );

    public abstract Task HandlePermanentsChoice(
        Permanent[] choices,
        Player player,
        Permanent[] options,
        int min,
        int max,
        string hint
    );

    public abstract Task HandleStringChoice(
        string? choice,
        Player player,
        string[] options,
        string hint
    );

    public abstract Task HandleCardChoice(
        Card? choice,
        Player player,
        Card[] options,
        string hint
    );

    public abstract Task HandleCostCollectionChoice(
        CostCollection? choice,
        Player player,
        CostCollection[] options,
        string hint
    );
    
    public abstract Task HandleManaPaymentChoice(
        IManaPaymentChoice choice,
        Player player,
        IManaPaymentChoice[] options,
        string hint
    );

    public abstract Task HandleAttackDeclarationsChoice(
        AttackDeclaration[] choices,
        Player player,
        AttackDeclaration[] options
    );

    public abstract Task HandleBlockDeclarationsChoice(
        BlockDeclaration[] choices,
        Player player,
        BlockDeclaration[] options
    );

    public void SetController(IPlayerController controller)
    {
        _controller = controller;
    }

    public IPlayerController GetWrappedController()
        => _controller;

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
        await _controller.Update(player);
    }

    public virtual Task HandleUpdate(Player player)
    {
        return Task.CompletedTask;
    }

    public async Task<(ICommand, RollbackRequest?)> ChooseCommand(Player player, ICommand[] options)
    {
        var result = await _controller.ChooseCommand(player, options);
        if (result.Item2 is null)
            await HandleCommandChoice(result.Item1, player, options);

        return result;
    }

    public async Task<(Card?, RollbackRequest?)> ChooseCard(Player player, Card[] options, string hint, bool allowNone)
    {
        var result = await _controller.ChooseCard(player, options, hint, allowNone);
        if (result.Item2 is null)
            await HandleCardChoice(result.Item1, player, options, hint);

        return result;
    }

    public async Task<(Player[], RollbackRequest?)> ChoosePlayers(
        Player player,
        Player[] options,
        int min,
        int max,
        string hint
    )
    {
        var result = await _controller.ChoosePlayers(player, options, min, max, hint);
        if (result.Item2 is null)
            await HandlePlayersChoice(result.Item1, player, options, min, max, hint);

        return result;
    }

    public async Task<(Permanent[], RollbackRequest?)> ChoosePermanents(
        Player player,
        Permanent[] options,
        int min,
        int max,
        string hint
    )
    {
        var result = await _controller.ChoosePermanents(player, options, min, max, hint);
        if (result.Item2 is null)
            await HandlePermanentsChoice(result.Item1, player, options, min, max, hint);

        return result;
    }

    public async Task<(string?, RollbackRequest?)> ChooseString(Player player, string[] options, string hint, bool allowNone)
    {
        var result = await _controller.ChooseString(player, options, hint, allowNone);
        if (result.Item2 is null)
            await HandleStringChoice(result.Item1, player, options, hint);

        return result;
    }
    

    public async Task<(CostCollection?, RollbackRequest?)> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
    {
        var result = await _controller.ChooseCostCollection(player, options, hint, allowNone);
        if (result.Item2 is null)
            await HandleCostCollectionChoice(result.Item1, player, options, hint);

        return result;
    }
    
    public async Task<(IManaPaymentChoice, RollbackRequest?)> ChooseManaPayment(Player player, IManaPaymentChoice[] options, string hint)
    {
        var result = await _controller.ChooseManaPayment(player, options, hint);
        if (result.Item2 is null)
            await HandleManaPaymentChoice(result.Item1, player, options, hint);

        return result;
    }

    public Task Update(Player player, string? stateMsg = null)
    {
        return _controller.Update(player, stateMsg);
    }

    public async Task<(AttackDeclaration[], RollbackRequest?)> ChooseAttackDeclarations(Player player, AttackDeclaration[] options)
    {
        var result = await _controller.ChooseAttackDeclarations(player, options);

        if (result.Item2 is null)
            await HandleAttackDeclarationsChoice(result.Item1, player, options);

        return result;
    }

    public async Task<(BlockDeclaration[], RollbackRequest?)> ChooseBlockDeclarations(Player player, BlockDeclaration[] options)
    {
        var result = await _controller.ChooseBlockDeclarations(player, options);
        
        if (result.Item2 is null)
            await HandleBlockDeclarationsChoice(result.Item1, player, options);

        return result;
    }

    public async Task<bool> ApproveRollback(Player player, string hint)
    {
        return await _controller.ApproveRollback(player, hint);
    }
}
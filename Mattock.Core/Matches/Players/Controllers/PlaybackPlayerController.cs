using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Players.Controllers;

public class PlaybackPlayerController(
    PlayerResponsesRecord record
) : IPlayerController
{
    public PlayerResponsesRecord Record { get; } = record.Clone();

    public Task<bool> ApproveRollback(Player player, string hint)
    {
        return Task.FromResult(true);
    }

    public Task<(AttackDeclaration[], RollbackRequest?)> ChooseAttackDeclarations(Player player, AttackDeclaration[] options)
    {
        throw new NotImplementedException();
    }

    public Task<(BlockDeclaration[], RollbackRequest?)> ChooseBlockDeclarations(Player player, BlockDeclaration[] options)
    {
        throw new NotImplementedException();
    }

    public Task<(Card?, RollbackRequest?)> ChooseCard(Player player, Card[] options, string hint, bool allowNone)
    {
        if (!record.CardChoices.TryDequeue(out var id))
        {
            throw new Exception($"Empty queue for {nameof(ChooseCard)}"); // TODO type
        }
        
        return Task.FromResult<(Card?, RollbackRequest?)>((
            options.Single(o => o.Id == id),
            null
        ));
    }

    public Task<(ICommand, RollbackRequest?)> ChooseCommand(Player player, ICommand[] options)
    {
        if (!record.CommandChoices.TryDequeue(out var s))
        {
            throw new Exception($"Empty queue for {nameof(ChooseCommand)}"); // TODO type
        }

        return Task.FromResult<(ICommand, RollbackRequest?)>((
            options.Single(o => o.ToCommandString() == s),
            null
        ));
    }

    public Task<(CostCollection?, RollbackRequest?)> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
    {
        if (!record.CostCollectionChoices.TryDequeue(out var text))
        {
            throw new Exception($"Empty queue for {nameof(ChooseCostCollection)}"); // TODO type
        }
        
        return Task.FromResult<(CostCollection?, RollbackRequest?)>((
            options.Single(o => o.Text == text), // TODO sus
            null
        ));
    }

    public Task<(IManaPaymentChoice, RollbackRequest?)> ChooseManaPayment(Player player, IManaPaymentChoice[] options, string hint)
    {
        if (!record.ManaPaymentChoices.TryDequeue(out var text))
        {
            throw new Exception($"Empty queue for {nameof(ChooseManaPayment)}"); // TODO type
        }
        
        return Task.FromResult<(IManaPaymentChoice, RollbackRequest?)>((
            options.Single(o => o.ToDisplayString() == text),
            null
        ));
    }

    public Task<(Permanent[], RollbackRequest?)> ChoosePermanents(Player player, Permanent[] options, int min, int max, string hint)
    {
        if (!record.PermanentsChoices.TryDequeue(out var pids))
        {
            throw new Exception($"Empty queue for {nameof(ChoosePermanents)}"); // TODO type
        }
        
        return Task.FromResult<(Permanent[], RollbackRequest?)>((
            [.. options.Where(o => pids.Contains(o.Pid))],
            null
        ));
    }

    public Task<(Player[], RollbackRequest?)> ChoosePlayers(Player player, Player[] options, int min, int max, string hint)
    {
        if (!record.PlayersChoices.TryDequeue(out var indicies))
        {
            throw new Exception($"Empty queue for {nameof(ChoosePlayers)}"); // TODO type
        }
        
        return Task.FromResult<(Player[], RollbackRequest?)>((
            [.. options.Where(o => indicies.Contains(o.Idx))],
            null
        ));
    }

    public Task<(string?, RollbackRequest?)> ChooseString(Player player, string[] options, string hint, bool allowNone)
    {
        if (!record.StringChoices.TryDequeue(out var text))
        {
            throw new Exception($"Empty queue for {nameof(ChooseString)}"); // TODO type
        }
        
        return Task.FromResult<(string?, RollbackRequest?)>((
            options.Single(o => o == text),
            null
        ));
    }

    public Task Update(Player player, string? stateMsg = null)
    {
        return Task.CompletedTask;
    }
}
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public Task<(ICommand, RollbackRequest?)> ChooseCommand(Player player, ICommand[] options)
    {
        throw new NotImplementedException();
    }

    public Task<(CostCollection?, RollbackRequest?)> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
    {
        throw new NotImplementedException();
    }

    public Task<(IManaPaymentChoice, RollbackRequest?)> ChooseManaPayment(Player player, IManaPaymentChoice[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<(Permanent[], RollbackRequest?)> ChoosePermanents(Player player, Permanent[] options, int min, int max, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<(Player[], RollbackRequest?)> ChoosePlayers(Player player, Player[] options, int min, int max, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<(string?, RollbackRequest?)> ChooseString(Player player, string[] options, string hint, bool allowNone)
    {
        throw new NotImplementedException();
    }

    public Task Update(Player player, string? stateMsg = null)
    {
        throw new NotImplementedException();
    }
}
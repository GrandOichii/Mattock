using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Matches.Players.Controllers;

public interface IPlayerController
{
    Task Update(Player player, string? stateMsg = null);

    Task<(ICommand, RollbackRequest?)> ChooseCommand(Player player, ICommand[] options);

    Task<(Player[], RollbackRequest?)> ChoosePlayers(
        Player player,
        Player[] options,
        int min,
        int max,
        string hint
    );

    Task<(Permanent[], RollbackRequest?)> ChoosePermanents(
        Player player,
        Permanent[] options,
        int min,
        int max,
        string hint
    );

    Task<(IManaPaymentChoice, RollbackRequest?)> ChooseManaPayment(
        Player player,
        IManaPaymentChoice[] options,
        string hint
    );

    Task<(string?, RollbackRequest?)> ChooseString(
        Player player,
        string[] options,
        string hint,
        bool allowNone
    );

    Task<(Card?, RollbackRequest?)> ChooseCard(
        Player player,
        Card[] options,
        string hint,
        bool allowNone
    );

    Task<(CostCollection?, RollbackRequest?)> ChooseCostCollection(
        Player player,
        CostCollection[] options,
        string hint,
        bool allowNone
    );

    Task<(AttackDeclaration[], RollbackRequest?)> ChooseAttackDeclarations(
        Player player,
        AttackDeclaration[] options
    );


    Task<(BlockDeclaration[], RollbackRequest?)> ChooseBlockDeclarations(
        Player player,
        BlockDeclaration[] options
    );

    Task<bool> ApproveRollback(
        Player player,
        string hint
    );
}

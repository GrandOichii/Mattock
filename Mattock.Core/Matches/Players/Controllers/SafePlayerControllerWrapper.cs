using Mattock.Core.Matches.Combat;
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;

namespace Mattock.Core.Matches.Players.Controllers;

public class SafePlayerControllerWrapper(
    IPlayerController controller
) : PlayerControllerWrapper(controller)
{
    public override Task HandleCardChoice(Card? choice, Player player, Card[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        
        if (!options.Contains(choice))
            throw new SafePlayerControllerWrapperException($"Controller chose card {choice.GetDisplayName()} for {nameof(ChooseCard)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleCommandChoice(ICommand choice, Player player, ICommand[] options)
    {
        if (!options.Select(c => c.ToCommandString()).Contains(choice.ToCommandString()))
            throw new SafePlayerControllerWrapperException($"Controller chose command \"{choice.ToCommandString()}\" for {nameof(ChooseCommand)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c.ToCommandString()}\""))})");
        return Task.CompletedTask;
    }

    public override Task HandleCostCollectionChoice(CostCollection? choice, Player player, CostCollection[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new SafePlayerControllerWrapperException($"Controller chose cost collection \"{choice.Text}\" for {nameof(ChooseCostCollection)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c.Text}\""))})");
        return Task.CompletedTask;
    }
    
    public override Task HandleManaPaymentChoice(IManaPaymentChoice? choice, Player player, IManaPaymentChoice[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new SafePlayerControllerWrapperException($"Controller chose mana payment {choice} for {nameof(ChooseManaPayment)}, which is not one of the options (options: {string.Join(", ", options.Select(o => o.ToDisplayString()))})");
        return Task.CompletedTask;
    }

    public override Task HandlePlayersChoice(Player[] choices, Player player, Player[] options, int min, int max, string hint)
    {
        if (choices.Length < min)
            throw new SafePlayerControllerWrapperException($"Controller chose {choices.Length} players for {nameof(ChoosePlayers)}, while min = {min}");
        if (max != -1 && choices.Length > max)
            throw new SafePlayerControllerWrapperException($"Controller chose {choices.Length} players for {nameof(ChoosePlayers)}, while max = {max}");
        
        Player[] badChoices = [.. choices.Where(c => !options.Contains(c))];
        if (badChoices.Length > 0)
            throw new SafePlayerControllerWrapperException($"Controller chose players {string.Join(", ", badChoices.Select(c => c.GetDisplayName()))} for {nameof(ChoosePlayers)}, which are not in options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandlePermanentsChoice(Permanent[] choices, Player player, Permanent[] options, int min, int max, string hint)
    {
        if (choices.Length < min)
            throw new SafePlayerControllerWrapperException($"Controller chose {choices.Length} permanents for {nameof(ChoosePermanents)}, while min = {min}");
        if (max != -1 && choices.Length > max)
            throw new SafePlayerControllerWrapperException($"Controller chose {choices.Length} permanents for {nameof(ChoosePermanents)}, while max = {max}");
        
        Permanent[] badChoices = [.. choices.Where(c => !options.Contains(c))];
        if (badChoices.Length > 0)
            throw new SafePlayerControllerWrapperException($"Controller chose permanents {string.Join(", ", badChoices.Select(c => c.GetDisplayName()))} for {nameof(ChoosePermanents)}, which are not in options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleStringChoice(string? choice, Player player, string[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new SafePlayerControllerWrapperException($"Controller chose string \"{choice}\" for {nameof(ChooseString)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c}\""))})");
        return Task.CompletedTask;
    }

    public override Task HandleAttackDeclarationsChoice(AttackDeclaration[] choices, Player player, AttackDeclaration[] options)
    {
        AttackDeclaration[] badChoices = [.. choices.Where(c => !options.Contains(c))];
        if (badChoices.Length > 0)
            throw new SafePlayerControllerWrapperException($"Controller chose attack declarations {string.Join(", ", badChoices.Select(c => c.GetDisplayName()))} for {nameof(ChooseAttackDeclarations)}, which are not in options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleBlockDeclarationsChoice(BlockDeclaration[] choices, Player player, BlockDeclaration[] options)
    {
        BlockDeclaration[] badChoices = [.. choices.Where(c => !options.Contains(c))];
        if (badChoices.Length > 0)
            throw new SafePlayerControllerWrapperException($"Controller chose block declarations {string.Join(", ", badChoices.Select(c => c.GetDisplayName()))} for {nameof(ChooseBlockDeclarations)}, which are not in options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }
}

// TODO docs
[Serializable]
public class SafePlayerControllerWrapperException : MatchException
{
    public SafePlayerControllerWrapperException() { }
    public SafePlayerControllerWrapperException(string message) : base(message) { }
}
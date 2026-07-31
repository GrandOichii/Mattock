using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Players.Controllers;

public class SafePlayerControllerWrapper(IPlayerController controller)
    : PlayerControllerWrapper(controller)
{
    public override Task HandleCardChoice(Card? choice, Player player, Card[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        
        if (!options.Contains(choice))
            throw new Exception($"Controller chose card {choice.GetDisplayName()} for {nameof(ChooseCard)}, which is not one of the options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleCommandChoice(ICommand choice, Player player, ICommand[] options)
    {
        if (!options.Select(c => c.ToCommandString()).Contains(choice.ToCommandString()))
            throw new Exception($"Controller chose command \"{choice.ToCommandString()}\" for {nameof(ChooseCommand)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c.ToCommandString()}\""))})");
        return Task.CompletedTask;
    }

    public override Task HandleCostCollectionChoice(CostCollection? choice, Player player, CostCollection[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new Exception($"Controller chose cost collection \"{choice.Text}\" for {nameof(ChooseCostCollection)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c.Text}\""))})");
        return Task.CompletedTask;
    }
    
    public override Task HandleStoredManaChoice(StoredMana? choice, Player player, StoredMana[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new Exception($"Controller chose stored mana {choice.Type} for {nameof(ChooseStoredMana)}, which is not one of the options (options: {string.Join(", ", options)})");
        return Task.CompletedTask;
    }

    public override Task HandlePlayersChoice(Player[] choices, Player player, Player[] options, int min, int max, string hint)
    {
        if (choices.Length < min)
            throw new Exception($"Controller chose {choices.Length} players for {nameof(ChoosePlayers)}, while min = {min}");
        if (max != -1 && choices.Length > max)
            throw new Exception($"Controller chose {choices.Length} players for {nameof(ChoosePlayers)}, while max = {max}");
        
        Player[] badChoices = [.. choices.Where(c => !options.Contains(c))];
        if (badChoices.Length > 0)
            throw new Exception($"Controller chose players {string.Join(", ", badChoices.Select(c => c.GetDisplayName()))} for {nameof(ChoosePlayers)}, which are not in options (options: {string.Join(", ", options.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleStringChoice(string? choice, Player player, string[] options, string hint)
    {
        if (choice is null) return Task.CompletedTask;
        if (!options.Contains(choice))
            throw new Exception($"Controller chose string \"{choice}\" for {nameof(ChooseString)}, which is not one of the options (options: {string.Join(", ", options.Select(c => $"\"{c}\""))})");
        return Task.CompletedTask;
    }
}
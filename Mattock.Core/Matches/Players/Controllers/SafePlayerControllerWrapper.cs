using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Players.Mana;

namespace Mattock.Core.Matches.Players.Controllers;

public class SafePlayerControllerWrapper(IPlayerController controller)
    : PlayerControllerWrapper(controller)
{
    public override Task HandleCardChoice(Card choice, Player player, Card[] choices, string hint)
    {
        if (!choices.Contains(choice))
            throw new Exception($"Controller chose card {choice.GetDisplayName()} for {nameof(ChooseCard)}, which is not one of the options (options: {string.Join(", ", choices.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleCommandChoice(ICommand choice, Player player, ICommand[] choices)
    {
        if (!choices.Select(c => c.ToCommandString()).Contains(choice.ToCommandString()))
            throw new Exception($"Controller chose command \"{choice.ToCommandString()}\" for {nameof(ChooseCommand)}, which is not one of the options (options: {string.Join(", ", choices.Select(c => $"\"{c.ToCommandString()}\""))})");
        return Task.CompletedTask;
    }

    public override Task HandleCostCollectionChoice(CostCollection choice, Player player, CostCollection[] choices, string hint)
    {
        if (!choices.Contains(choice))
            throw new Exception($"Controller chose cost collection \"{choice.Text}\" for {nameof(ChooseCostCollection)}, which is not one of the options (options: {string.Join(", ", choices.Select(c => $"\"{c.Text}\""))})");
        return Task.CompletedTask;
    }
    
    public override Task HandleStoredManaChoice(StoredMana choice, Player player, StoredMana[] choices, string hint)
    {
        if (!choices.Contains(choice))
            throw new Exception($"Controller chose stored mana {choice.Type} for {nameof(ChooseStoredMana)}, which is not one of the options (options: {string.Join(", ", choices)})");
        return Task.CompletedTask;
    }

    public override Task HandlePlayerChoice(Player choice, Player player, Player[] choices, string hint)
    {
        if (!choices.Contains(choice))
            throw new Exception($"Controller chose player {choice.GetDisplayName()} for {nameof(ChoosePlayer)}, which is not one of the options (options: {string.Join(", ", choices.Select(c => c.GetDisplayName()))})");
        return Task.CompletedTask;
    }

    public override Task HandleStringChoice(string choice, Player player, string[] choices, string hint)
    {
        if (!choices.Contains(choice))
            throw new Exception($"Controller chose string \"{choice}\" for {nameof(ChooseString)}, which is not one of the options (options: {string.Join(", ", choices.Select(c => $"\"{c}\""))})");
        return Task.CompletedTask;
    }
}
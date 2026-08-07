using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Players.Controllers;

public class RecorderPlayerControllerWrapper(IPlayerController controller)
    : PlayerControllerWrapper(controller)
{
    public PlayerResponsesRecord Record { get; } = new();

    public override Task HandleCardChoice(Card? choice, Player player, Card[] options, string hint)
    {
        Record.CardChoices.Add(choice?.Id);
        return Task.CompletedTask;
    }
    

    public override Task HandleCommandChoice(ICommand choice, Player player, ICommand[] choices)
    {
        Record.CommandChoices.Add(choice.ToCommandString());
        return Task.CompletedTask;
    }
    

    public override Task HandleCostCollectionChoice(CostCollection? choice, Player player, CostCollection[] options, string hint)
    {
        Record.CostCollectionChoices.Add(choice?.Text); // TODO kinda sus, are all texts unique?
        return Task.CompletedTask;
    }
    

    public override Task HandleManaPaymentChoice(IManaPaymentChoice choice, Player player, IManaPaymentChoice[] options, string hint)
    {
        Record.ManaPaymentChoices.Add(choice.ToDisplayString());
        return Task.CompletedTask;
    }
    

    public override Task HandlePermanentsChoice(Permanent[] choices, Player player, Permanent[] options, int min, int max, string hint)
    {
        Record.PermanentsChoices.Add([.. choices.Select(p => p.Pid)]);
        return Task.CompletedTask;
    }
    

    public override Task HandlePlayersChoice(Player[] choices, Player player, Player[] options, int min, int max, string hint)
    {
        Record.PlayersChoices.Add([.. choices.Select(p => p.Idx)]);
        return Task.CompletedTask;
    }
    

    public override Task HandleStringChoice(string? choice, Player player, string[] options, string hint)
    {
        Record.StringChoices.Add(choice);
        return Task.CompletedTask;
    }
    
}
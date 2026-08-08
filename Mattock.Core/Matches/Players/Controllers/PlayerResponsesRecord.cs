namespace Mattock.Core.Matches.Players.Controllers;


public class PlayerResponsesRecord
{
    public Queue<string?> CardChoices { get; init; } = [];
    public Queue<string> CommandChoices { get; init; } = [];
    public Queue<string?> CostCollectionChoices { get; init; } = [];
    public Queue<string> ManaPaymentChoices { get; init; } = [];

    public Queue<string[]> PermanentsChoices { get; init; } = [];
    public Queue<int[]> PlayersChoices { get; init; } = [];
    public Queue<string?> StringChoices { get; init; } = [];

    public PlayerResponsesRecord Clone()
        => new()
        {
            CardChoices = new([.. CardChoices]),
            CommandChoices = new([.. CommandChoices]),
            CostCollectionChoices = new([.. CostCollectionChoices]),
            ManaPaymentChoices = new([.. ManaPaymentChoices]),
            PermanentsChoices = new([.. PermanentsChoices]),
            PlayersChoices = new([.. PlayersChoices]),
            StringChoices = new([.. StringChoices]),
        };
}
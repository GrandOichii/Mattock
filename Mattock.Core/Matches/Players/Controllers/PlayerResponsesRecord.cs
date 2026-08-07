namespace Mattock.Core.Matches.Players.Controllers;


public class PlayerResponsesRecord
{
    public List<string?> CardChoices { get; init; } = [];
    public List<string> CommandChoices { get; init; } = [];
    public List<string?> CostCollectionChoices { get; init; } = [];
    public List<string> ManaPaymentChoices { get; init; } = [];

    public List<string[]> PermanentsChoices { get; init; } = [];
    public List<int[]> PlayersChoices { get; init; } = [];
    public List<string?> StringChoices { get; init; } = [];

    public PlayerResponsesRecord Clone()
        => new()
        {
            CardChoices = [.. CardChoices],
            CommandChoices = [.. CommandChoices],
            CostCollectionChoices = [.. CostCollectionChoices],
            ManaPaymentChoices = [.. ManaPaymentChoices],
            PermanentsChoices = [.. PermanentsChoices],
            PlayersChoices = [.. PlayersChoices],
            StringChoices = [.. StringChoices],
        };
}
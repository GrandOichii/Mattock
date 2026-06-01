namespace Mattock.Core.Setup;

public class MatchConfig
{
    public required bool RandomMatch { get; set; }
    public required int Seed { get; set; }
    public required bool RandomFirstPlayer { get; set; }
    public required int FirstPlayerIdx { get; set; }
    public required int StartingLifeTotal { get; set; }
    public required int InitialHandSize { get; set; }
    public required bool GameLossIfRequiredToDrawFromEmptyLibrary { get; set; }
    public required int DrawStepDrawAmount { get; set; }
    public required bool FirstPlayerNoDrawIfSingleOpponent { get; set; }
    public required int MaxHandSize { get; set; }
    public required int MaxLandsPerTurn { get; set; }

    public readonly static MatchConfig Default = new()
    {
        FirstPlayerIdx = -1,
        RandomFirstPlayer = true,
        Seed = -1,
        RandomMatch = true,
        StartingLifeTotal = 20,
        InitialHandSize = 7,
        GameLossIfRequiredToDrawFromEmptyLibrary = true,
        DrawStepDrawAmount = 1,
        FirstPlayerNoDrawIfSingleOpponent = true,
        MaxHandSize = 7,
        MaxLandsPerTurn = 1,
    };

    public static MatchConfig Copy(MatchConfig config) => new()
    {
        FirstPlayerIdx = config.FirstPlayerIdx,
        Seed = config.Seed,
        RandomFirstPlayer = config.RandomFirstPlayer,
        RandomMatch = config.RandomMatch,
        StartingLifeTotal = config.StartingLifeTotal,
        InitialHandSize = config.InitialHandSize,
        GameLossIfRequiredToDrawFromEmptyLibrary = config.GameLossIfRequiredToDrawFromEmptyLibrary,
        DrawStepDrawAmount = config.DrawStepDrawAmount,
        FirstPlayerNoDrawIfSingleOpponent = config.FirstPlayerNoDrawIfSingleOpponent,
        MaxHandSize = config.MaxHandSize,
        MaxLandsPerTurn = config.MaxLandsPerTurn,
    };
}
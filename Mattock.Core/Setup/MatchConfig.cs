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
    public required bool GameLossIfZeroOrLessLife { get; set; }
    public required int DrawStepDrawAmount { get; set; }
    public required bool FirstPlayerNoDrawIfSingleOpponent { get; set; }
    public required int MaxHandSize { get; set; }
    public required int MaxLandsPerTurn { get; set; }
    public required bool ManaPoolEmptiesAtEndOfEachPhase { get; set; }
    public required bool ManaPoolEmptiesAtEndOfEachStep { get; set; }
    public required int TeamCount { get; set; }
    public required int MaxTeamSize { get; set; }

    public readonly static MatchConfig Default = new()
    {
        FirstPlayerIdx = -1,
        RandomFirstPlayer = true,
        Seed = -1,
        RandomMatch = true,
        StartingLifeTotal = 20,
        InitialHandSize = 7,
        GameLossIfRequiredToDrawFromEmptyLibrary = true,
        GameLossIfZeroOrLessLife = true,
        DrawStepDrawAmount = 1,
        FirstPlayerNoDrawIfSingleOpponent = true,
        MaxHandSize = 7,
        MaxLandsPerTurn = 1,
        ManaPoolEmptiesAtEndOfEachPhase = true,
        ManaPoolEmptiesAtEndOfEachStep = true,
        TeamCount = 4,
        MaxTeamSize = 1,
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
        GameLossIfZeroOrLessLife = config.GameLossIfZeroOrLessLife,
        DrawStepDrawAmount = config.DrawStepDrawAmount,
        FirstPlayerNoDrawIfSingleOpponent = config.FirstPlayerNoDrawIfSingleOpponent,
        MaxHandSize = config.MaxHandSize,
        MaxLandsPerTurn = config.MaxLandsPerTurn,
        ManaPoolEmptiesAtEndOfEachPhase = config.ManaPoolEmptiesAtEndOfEachPhase,
        ManaPoolEmptiesAtEndOfEachStep = config.ManaPoolEmptiesAtEndOfEachStep,
        TeamCount = config.TeamCount,
        MaxTeamSize = config.MaxTeamSize,
    };
}
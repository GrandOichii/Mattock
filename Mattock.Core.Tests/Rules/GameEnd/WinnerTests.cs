using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.GameEnd;

/// <summary>
/// These tests check which players won the match
/// </summary>
public class WinnerTests
{
    public readonly static IEnumerable<TheoryDataRow<(int, PlayerStatus)[], int[], int[]>> WinnersDecided_Data = [
        // * 2 teams of 1 player
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Won )
            ],
            [ 1 ],
            [ 1 ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost )
            ],
            [ 0 ],
            [ 0 ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.Lost )
            ],
            [ ],
            [ ]
        ),
        new(
            [
                (0, PlayerStatus.Won ),
                (1, PlayerStatus.Won )
            ],
            [ 0, 1 ],
            [ 0, 1 ]
        ),
        // * 2 teams, first: 1 player, second: 2 players
        new(
            [
                (0, PlayerStatus.Won ),
                (1, PlayerStatus.Won ),
                (1, PlayerStatus.Won ),
            ],
            [ 0, 1 ],
            [ 0, 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.Won ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
            ],
            [ 0 ],
            [ 0 ]
        ),
        new(
            [
                (0, PlayerStatus.Won ),
                (1, PlayerStatus.Won ),
                (1, PlayerStatus.InGame ),
            ],
            [ 0, 1 ],
            [ 0, 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.Won ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Won ),
            ],
            [ 0, 1 ],
            [ 0, 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Won ),
                (1, PlayerStatus.Won ),
            ],
            [ 1 ],
            [ 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Won ),
                (1, PlayerStatus.InGame ),
            ],
            [ 1 ],
            [ 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Won ),
            ],
            [ 1 ],
            [ 1, 2 ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.Lost ),
            ],
            [ ],
            [ ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.Lost ),
            ],
            [ 0 ],
            [ 0 ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
            ],
            [ 1 ],
            [ 1, 2 ]
        ),
    ];

    [Theory]
    [MemberData(nameof(WinnersDecided_Data))]
    public async Task WinnersDecided((int, PlayerStatus)[] players, int[] expectedWinningTeams, int[] expectedWinningIndicies)
    {
        // Arrange
        var pCount = players.Length;
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck);

        for (int i = 0; i < pCount; ++i)
            p1.Act.SetPlayerStatus(i, players[i].Item2, true);

        p1
            .Act.CheckForWinners()
            .Act.Pass();

        TestPlayerControllerBuilder createPlayer(int idx, int tIdx) => idx == 0
            ? p1
            : new TestPlayerControllerBuilder($"p{idx+1}", tIdx)
                .SetDeck(deck);

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(0)
                .MaxTeamSize(10)
                .TeamCount(10)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [
                .. players.Select(
                    (p, idx) => createPlayer(idx, p.Item1)
                )
            ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .DidntCrash()
            .NoChoicesLeft()
            .CurrentPhase(PhaseType.Beginning)
            .CurrentStep(StepType.Upkeep)
            .WinningTeams(expectedWinningTeams)
        );

        for (int i = 0; i < pCount; ++i)
        {
            if (expectedWinningIndicies.Contains(i))
                match.Assert(a => a.AssertPlayer(i, pa => pa.Won()));
            else
                match.Assert(a => a.AssertPlayer(i, pa => pa.Lost()));
        }
    }

    public readonly static IEnumerable<TheoryDataRow<(int, PlayerStatus)[]>> NoWinnersDecided_Data = [
        // * 2 teams of 1 player
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame )
            ]
        ),
        // * 2 teams, first: 1 player, second: 2 players
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
            ]
        ),
        // * 2 teams of 2 players
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame )
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
            ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame ),
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
            ]
        ),
        new(
            [
                (0, PlayerStatus.Lost ),
                (0, PlayerStatus.InGame ),
                (1, PlayerStatus.InGame ),
                (1, PlayerStatus.Lost ),
            ]
        ),
        new(
            [
                (0, PlayerStatus.InGame ),
                (0, PlayerStatus.Lost ),
                (1, PlayerStatus.Lost ),
                (1, PlayerStatus.InGame ),
            ]
        ),
    ];

    [Theory]
    [MemberData(nameof(NoWinnersDecided_Data))]
    public async Task NoWinnersDecided((int, PlayerStatus)[] players)
    {
        // Arrange
        var pCount = players.Length;
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck);

        for (int i = 0; i < pCount; ++i)
            p1.Act.SetPlayerStatus(i, players[i].Item2, true);

        p1
            .Act.CheckForWinners()
            .Act.Crash();

        TestPlayerControllerBuilder createPlayer(int idx, int tIdx) => idx == 0
            ? p1
            : new TestPlayerControllerBuilder($"p{idx+1}", tIdx)
                .SetDeck(deck);

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(0)
                .MaxTeamSize(10)
                .TeamCount(10)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [
                .. players.Select(
                    (p, idx) => createPlayer(idx, p.Item1)
                )
            ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .CurrentPhase(PhaseType.Beginning)
            .CurrentStep(StepType.Upkeep)
            .NoWinnersDecided()
        );
    }
}
namespace Mattock.Core.Tests.Rules.Teams;

/// <summary>
/// These tests check whether a game can start usiong the provided match configuration and players
/// </summary>
public class TeamValidationTests
{
    private static Dictionary<int, int> ParseFormattedTeamSizes(string s)
    {
        return s.Split(" ").ToDictionary(
            i => int.Parse(i.Split(":")[0]),
            i => int.Parse(i.Split(":")[1])
        );
    }

    [Theory]    
    [InlineData(2, 1, "0:1 1:1")] // 2 teams of 1 player
    [InlineData(4, 1, "0:1 1:1 2:1 3:1")] // 4 teams of 1 player
    [InlineData(2, 2, "0:2 1:2")] // 2 teams of 2 players
    [InlineData(2, 2, "0:2 1:1")] // 2 teams with inbalanced players
    [InlineData(3, 1, "0:1 1:1 2:0")] // 3 teams with one team absent
    public async Task CanStart(int teamCount, int maxTeamSize, string formattedTeamSizes)
    {
        // Arrange
        List<PlayerSetup> playerSetups = [];
        var teamSizes = ParseFormattedTeamSizes(formattedTeamSizes);
        int p = 0;
        foreach (var (teamIdx, count) in teamSizes)
        {
            for (int i = 0; i < count; ++i)
            {
                playerSetups.Add(new()
                {
                    Controller = null!,
                    Deck = null!,
                    Name = $"p{p++}",
                    TeamIdx = teamIdx
                });
            }
        }

        // Act
        var act = () => new Match(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .TeamCount(teamCount)
                .MaxTeamSize(maxTeamSize)
                .Build(),
            [ .. playerSetups ],
            new()
            {
                Mulligan = null
            }
        );

        // Assert
        act.ShouldNotThrow();
    }

    [Theory]    
    [InlineData(2, 1, "0:1 1:2", typeof(TeamTooBigException))]
    [InlineData(2, 1, "0:2 1:1", typeof(TeamTooBigException))]
    [InlineData(2, 1, "0:1 1:1 2:1", typeof(TooManyTeamsException))]
    public async Task CantStart(int teamCount, int maxTeamSize, string formattedTeamSizes, Type type)
    {
        // Arrange
        List<PlayerSetup> playerSetups = [];
        var teamSizes = ParseFormattedTeamSizes(formattedTeamSizes);
        int p = 0;
        foreach (var (teamIdx, count) in teamSizes)
        {
            for (int i = 0; i < count; ++i)
            {
                playerSetups.Add(new()
                {
                    Controller = null!,
                    Deck = null!,
                    Name = $"p{p++}",
                    TeamIdx = teamIdx
                });
            }
        }

        // Act
        var act = () => new Match(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .TeamCount(teamCount)
                .MaxTeamSize(maxTeamSize)
                .Build(),
            [ .. playerSetups ],
            new()
            {
                Mulligan = null
            }
        );

        // Assert
        act.ShouldThrow(type);
    }

    [Fact]
    public async Task CantStart_SamePlayerName()
    {
        // Arrange

        // Act
        var act = () => new Match(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [
                new() {
                    Controller = null!,
                    Deck = null!,
                    Name = "name",
                    TeamIdx = 0
                },
                new() {
                    Controller = null!,
                    Deck = null!,
                    Name = "name",
                    TeamIdx = 1
                },
            ],
            new()
            {
                Mulligan = null
            }
        );

        // Assert
        act.ShouldThrow<DuplicatePlayerNameException>();
    }
}
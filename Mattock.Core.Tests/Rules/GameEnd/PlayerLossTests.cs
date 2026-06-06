namespace Mattock.Core.Tests.Rules.GameEnd;

/// <summary>
/// These tests check that players that lost the game aren't able to do any actions
/// </summary>
public class PlayerLossTests
{
    [Trait("Rules", "104.2.,104.3.")]
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task KillPlayerAndSkipToFirstPlayerNextTurn(int oppIdx)
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // upkeep
            .Act.SetLife(oppIdx, 0)
            .Act.AutoPassToTurn(4)
            .Act.Crash()
            ;

        TestPlayerControllerBuilder activeNonP1(string name, int teamIdx) => new TestPlayerControllerBuilder(name, teamIdx)
            .SetDeck(deck)
            .Act.AutoPass()
            ;
        

        TestPlayerControllerBuilder nonactiveNonP1(string name, int teamIdx) => new TestPlayerControllerBuilder(name, teamIdx)
            .SetDeck(deck)
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [ 
                p1, 
                .. Enumerable.Range(1, 3)
                    .Select(i => i == oppIdx ? nonactiveNonP1($"p{i+1}", i) : activeNonP1($"p{i+1}", i))
            ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert

        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(oppIdx, p => p
                .Lost()
            )
        );
    }
}
namespace Mattock.Core.Tests.Rules.Costs;

// 118.2.
// 601.2f
// 601.2g
// 601.2h

public class ManaCostTests
{
    [Fact]
    [Trait("Rules", "118.2.,601.2f,601.2g,601.2h")]
    public async Task TODORenameMe()
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }
}
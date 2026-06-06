using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.StateBasedActions;

public class DrawFromEmptyLibraryTests
{
    [Fact]
    public async Task DefeatWhenAttemptToDrawFromEmptyLibrary()
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
            .Act.Pass()
            // upkeep: player gains priority and immediately loses the game
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(0)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .DidntCrash()
            .NoChoicesLeft()
            .CurrentPhase(PhaseType.Beginning)
            .CurrentStep(StepType.Draw)
            .WinningTeams([1])
        );
    }
}
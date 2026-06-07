namespace Mattock.Core.Tests.Rules.TurnStructure.Steps;

public class CleanupStepTests
{
    [Trait("Rules", "514.1.")]
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task DiscardToHandSize(int overMaxHandSize)
    {
        // Arrange
        var deckSize = 7 + overMaxHandSize;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder()
                    .Amount(deckSize)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.AutoPassToStep(StepType.End)
            .Act.Pass()
            .ChooseCard.NTimes(overMaxHandSize, (i, c) => c
                .First()
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .InitialHandSize(deckSize)
                .MaxHandSize(deckSize - overMaxHandSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, pa => pa
                .AssertLibrary(ha => ha.HasCardCount(0))
                .AssertHand(ha => ha.HasCardCount(7))
                .AssertGraveyard(ga => ga.HasCardCount(overMaxHandSize))
            )
        );
    }
}
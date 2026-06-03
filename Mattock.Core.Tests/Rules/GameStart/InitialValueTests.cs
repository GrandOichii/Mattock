namespace Mattock.Core.Tests.Rules.GameStart;

public class InitialValueTests
{
    [Theory]
    [Trait("Rules", "103,103.1.,103.4.,119.1.,103.3,103.5")]
    [InlineData(0)]
    [InlineData(1)]
    public async Task FirstPlayerPrompt_StartingLifeTotal_EmptyLibraries_EmptyManaPools(int firstPlayerIdx)
    {
        // Arrange
        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(firstPlayerIdx);

        var p2 = new TestPlayerControllerBuilder("p2");

        (firstPlayerIdx == 0
            ? p1
            : p2).Act.Crash();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .ActivePlayerIs(firstPlayerIdx)
            .AssertPlayer(0, ap => ap
                .HasLife(20)
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertManaPool(pa => pa
                    .IsEmpty()
                )
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertManaPool(pa => pa
                    .IsEmpty()
                )
            )
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(20)]
    [Trait("Rules", "103.3,103.5")]
    public async Task InitialHandSize(int initialHandSize)
    {
        // Arrange
        int deckSize = 20;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder().Amount(deckSize).Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .Act.Crash();

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck);

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(initialHandSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize - initialHandSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(initialHandSize)
                )
            )
            .AssertPlayer(1, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize - initialHandSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(initialHandSize)
                )
            )
        );
    }
}
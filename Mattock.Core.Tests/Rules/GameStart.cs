
using Mattock.Core.Setup.Templates;
using Mattock.Core.Tests.Setup;

namespace Mattock.Core.Tests.Rules;

public class GameStart
{
    [Theory]
    [Trait("Rules", "103,103.1.,103.4.,119.1.,103.3,103.5")]
    [InlineData(0)]
    [InlineData(1)]
    public async Task FirstPlayerPrompt_StartingLifeTotal_EmptyDecks(int firstPlayerIdx)
    {
        // Arrange
        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(firstPlayerIdx)
            .Build();

        var p2 = new TestPlayerControllerBuilder("p2")
            .Build();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ p1, p2 ]
        );

        await match.Run();

        // Assert
        match.Assert(a => a
            .NoChoicesLeft()
            .ActivePlayerIs(firstPlayerIdx)
            .AssertPlayer(0, ap => ap
                .HasLife(20)
                .AssertDeck(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
                .AssertDeck(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
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
                new() {
                    Amount = 20,
                    Card = new() {
                        ColorIndicator = [],
                        Defense = "",
                        HandModifier = "",
                        LifeModifier = "",
                        Loyalty = 0,
                        ManaCost = [],
                        Name = "c1",
                        Power = "",
                        Toughness = "",
                        TextBox = "",
                        TypeLine = ""
                    }
                }
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .Build();

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .Build();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(initialHandSize)
                .Build(),
            [ p1, p2 ]
        );

        await match.Run();

        // Assert
        match.Assert(a => a
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertDeck(ad => ad
                    .HasCardCount(deckSize - initialHandSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(initialHandSize)
                )
            )
            .AssertPlayer(1, ap => ap
                .AssertDeck(ad => ad
                    .HasCardCount(deckSize - initialHandSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(initialHandSize)
                )
            )
        );
    }
}
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mechanics.Mulligans;
using Mattock.Core.Tests.Setup.Asserts;

namespace Mattock.Core.Tests.Rules.GameStart;

public class LondonMulliganTests
{
    [Fact]
    [Trait("Rules", "103.5")]
    public async Task AllDecline()
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1").Amount(7).Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .ChooseString.No()
            // upkeep
            .Act.Crash();

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .ChooseString.No();

        var p3 = new TestPlayerControllerBuilder("p3")
            .SetDeck(deck)
            .ChooseString.No();

        var p4 = new TestPlayerControllerBuilder("p4")
            .SetDeck(deck)
            .ChooseString.No();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ p1, p2, p3, p4 ]
        );

        await match.Run();

        // Assert
        Action<PlayerAsserts> playerAsserts = ap => ap
            .AssertLibrary(ad => ad
                .HasCardCount(0)
            )
            .AssertHand(ad => ad
                .HasCardCount(7)
            );
            
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, playerAsserts)
            .AssertPlayer(1, playerAsserts)
            .AssertPlayer(2, playerAsserts)
            .AssertPlayer(3, playerAsserts)
        );
    }

    [Theory]
    [InlineData(0, 6)]
    [InlineData(1, 7)]
    [Trait("Rules", "103.5")]
    public async Task AllAcceptOnce_ThenDecline(int freeMulligans, int expectedHandSize)
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1").Amount(6).Build(),
                new DeckCardTemplateBuilder("c2").Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseString.No()
            // upkeep
            .Act.Crash();

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseString.No();

        var p3 = new TestPlayerControllerBuilder("p3")
            .SetDeck(deck)
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseString.No();

        var p4 = new TestPlayerControllerBuilder("p4")
            .SetDeck(deck)
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseString.No();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ p1, p2, p3, p4 ]
        );
        match.SetMulligan(new LondonMulliganRule(freeMulligans));

        await match.Run();

        // Assert
        Action<PlayerAsserts> playerAsserts = ap => ap
            .AssertLibrary(ad =>
            {
                var expectedLibrarySize = 7 - expectedHandSize;
                ad.HasCardCount(expectedLibrarySize);

                if (expectedLibrarySize == 0) return;
                ad.AssertCard(0, ac => ac
                    .HasName("c2")
                );

            })
            .AssertHand(ad => ad
                .HasCardCount(expectedHandSize)
            );

        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft(checkCardChoices: false)
            .AssertPlayer(0, playerAsserts)
            .AssertPlayer(1, playerAsserts)
            .AssertPlayer(2, playerAsserts)
            .AssertPlayer(3, playerAsserts)
        );
    }

    [Fact]
    [Trait("Rules", "103.5")]
    public async Task SingleMulligansTwice_OthersDecline()
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1").Amount(5).Build(),
                new DeckCardTemplateBuilder("c2").Build(),
                new DeckCardTemplateBuilder("c3").Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseString.Yes()
            .ChooseCard.FirstWithName("c2")
            .ChooseCard.FirstWithName("c3")
            .ChooseString.No()
            // upkeep
            .Act.Crash();

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .ChooseString.No();

        var p3 = new TestPlayerControllerBuilder("p3")
            .SetDeck(deck)
            .ChooseString.No();

        var p4 = new TestPlayerControllerBuilder("p4")
            .SetDeck(deck)
            .ChooseString.No();

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ p1, p2, p3, p4 ]
        );

        await match.Run();

        // Assert
        Action<PlayerAsserts> playerAsserts = ap => ap
            .AssertLibrary(ad => ad
                .HasCardCount(0)
            )
            .AssertHand(ad => ad
                .HasCardCount(7)
            );

        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(2)
                    .AssertCard(0, ac => ac
                        .HasName("c2")
                    )
                    .AssertCard(1, ac => ac
                        .HasName("c3")
                    )
                )
                .AssertHand(ad => ad
                    .HasCardCount(5)
                )
            )
            .AssertPlayer(1, playerAsserts)
            .AssertPlayer(2, playerAsserts)
            .AssertPlayer(3, playerAsserts)
        );
    }
}
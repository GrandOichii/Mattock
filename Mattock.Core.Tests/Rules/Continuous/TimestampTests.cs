namespace Mattock.Core.Tests.Rules.Continuous;

/// <summary>
/// Tests for rules 613.7a - 613.7n
/// </summary>
public class TimestampTests
{
    [Fact]
    [Trait("Rules", "613.7a")]
    public static async Task CheckInitialTimestamps()
    {
        // Arrange

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder()
                    .Artifact()
                    .ZeroCost()
                    .Amount(60)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        foreach (var card in cards)
                        {
                            card.Timestamp.ShouldBePositive();
                        }
                    })
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }

    [Fact]
    [Trait("Rules", "613.7a")]
    public static async Task CheckStackAndBattlefieldTimestamps()
    {
        // Arrange

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("a")
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        long timestamp = -1;

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        timestamp = card.Timestamp;
                    })
                )
            )
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldBe(timestamp);
                    })
                )
            )
            .Act.CastSpellWithName("a")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldNotBe(timestamp);
                        timestamp = card.Timestamp;
                    })
                )
            )
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldNotBe(timestamp);
                    })
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }
    
    [Fact]
    [Trait("Rules", "613.7a")]
    public static async Task CheckStackAndGraveyardTimestamps()
    {
        // Arrange

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("s")
                    .Sorcery()
                    .ZeroCost()
                    .Build(),
            ]
        };

        long timestamp = -1;

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        timestamp = card.Timestamp;
                    })
                )
            )
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldBe(timestamp);
                    })
                )
            )
            .Act.CastSpellWithName("s")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldNotBe(timestamp);
                        timestamp = card.Timestamp;
                    })
                )
            )
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldNotBe(timestamp);
                    })
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }

    [Fact]
    [Trait("Rules", "613.7a")]
    public static async Task CheckMillTimestamps()
    {
        // Arrange

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .InitialHandSize(0)
            .DrawStepDrawAmount(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("s")
                    .Sorcery()
                    .ZeroCost()
                    .Build(),
            ]
        };

        long timestamp = -1;

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        timestamp = card.Timestamp;
                    })
                )
            )
            .Act.Mill(0, 1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .Assert(match =>
                    {
                        var cards = match.GetCards();
                        var card = cards[0];
                        card.Timestamp.ShouldNotBe(timestamp);
                    })
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestSessionWrapper(
            config,
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        // Act
        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }
}
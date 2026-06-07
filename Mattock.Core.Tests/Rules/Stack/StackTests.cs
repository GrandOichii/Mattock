namespace Mattock.Core.Tests.Rules.Stack;

public class StackTests
{
    // TODO

    // Add test: Player A casts a spell, Player B casts

    [Trait("Rules", "117.3b,117.3b,117.3c,")]
    [Fact]
    public async Task InResponse()
    {
        // Arrange
        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(new DeckTemplate()
            {
                MainDeck = [
                    new DeckCardTemplateBuilder("c0-1")
                        .Instant()
                        .ZeroCost()
                        .Build(),
                    new DeckCardTemplateBuilder("c0-2")
                        .Instant()
                        .ZeroCost()
                        .Build(),
                ]
            })
            // * upkeep
            .Act.CastSpellWithName("c0-1")
            .Act.Assert(a => a
                .AssertMatch(ma => ma.AssertStack(sa => sa.EffectCount(1)))
            )
            .Act.CastSpellWithName("c0-2")
            .Act.Assert(a => a
                .AssertMatch(ma => ma.AssertStack(sa => sa.EffectCount(2)))
            )
            .Act.Pass()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(new DeckTemplate()
            {
                MainDeck = [
                    new DeckCardTemplateBuilder("c1-1")
                        .Instant()
                        .ZeroCost()
                        .Build(),
                ]
            })
            .Act.CastSpellWithName("c1-1")
            .Act.Crash()
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
            .AssertStack(sa => sa
                .EffectCount(3)
                .AssertEffect(0, ea => ea
                    .HasController(0)
                    .AssertAsSpell(sa => sa
                        .CardName("c0-1")
                    )
                )
                .AssertEffect(1, ea => ea
                    .HasController(0)
                    .AssertAsSpell(sa => sa
                        .CardName("c0-2")
                    )
                )
                .AssertEffect(2, ea => ea
                    .HasController(1)
                    .AssertAsSpell(sa => sa
                        .CardName("c1-1")
                    )
                )
            )
        );
    }

    // TODO MORE
}
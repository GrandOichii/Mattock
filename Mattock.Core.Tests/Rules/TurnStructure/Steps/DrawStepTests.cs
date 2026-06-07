
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.TurnStructure.Steps;

public class DrawStepTests
{
    [Theory]
    [Trait("Rules", "504.")]
    [InlineData(1, 7)]
    [InlineData(2, 8)]
    [InlineData(3, 8)]
    public async Task FirstDraw_ThenGainPriority(int opponentCount, int expectedHandSize)
    {
        // Arrange
        var deckSize = 8;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1").Amount(deckSize).Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .ChooseString.No()
            // upkeep
            .Act.Pass();
        if (expectedHandSize == 8)
        {
            p1.Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
            );
        } else
        {
            p1.Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                )
            );
        }

        p1.Act.Crash();

        TestPlayerControllerBuilder nonP1(string name, int teamIdx) => new TestPlayerControllerBuilder(name, teamIdx)
            .SetDeck(deck)
            .ChooseString.No()
            .Act.AutoPass()

            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .Build(),
            [ 
                p1, 
                .. Enumerable
                    .Range(0, opponentCount)
                    .Select(idx => nonP1($"p{idx+2}", idx+1))
            ]
        );

        await match.Run();

        // Assert
        void nonP1Asserts(PlayerAsserts ap) => ap
            .AssertLibrary(ad => ad
                .HasCardCount(1)
            )
            .AssertHand(ad => ad
                .HasCardCount(7)
            );

        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertHand(ad => ad
                    .HasCardCount(expectedHandSize)
                )
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize - expectedHandSize)
                )
            )
        );

        for (int i = 0; i < opponentCount; ++i)
        {
            match.Assert(a => a
                .AssertPlayer(i+1, nonP1Asserts)
            );
        }
    }
    
    [Theory]
    [Trait("Rules", "504.")]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 4)]
    [InlineData(8, 8)]
    public async Task MultipleTurns(int turnCount, int expectedHandSize)
    {
        // Arrange
        var deckSize = 8;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder().Amount(deckSize).Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .ChooseString.No()
            // upkeep
            .Act.AutoPassToTurn(4 * turnCount + 1)
            .Act.Crash();

        TestPlayerControllerBuilder nonP1(string name, int teamIdx) => new TestPlayerControllerBuilder(name, teamIdx)
            .SetDeck(deck)
            .ChooseString.No()
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .InitialHandSize(0)
                .NoMaxHandSize()
                .Build(),
            [ 
                p1, 
                .. Enumerable
                    .Range(0, 3)
                    .Select(idx => nonP1($"p{idx+2}", idx+1))
            ]
        );

        await match.Run();

        // Assert
        for (int i = 0; i < 4; ++i)
        {
            match.Assert(a => a
                .AssertPlayer(i, ap => ap
                    .AssertHand(ad => ad
                        .HasCardCount(expectedHandSize)
                    )
                    .AssertLibrary(ad => ad
                        .HasCardCount(deckSize - expectedHandSize)
                    )
                )
            );
        }
    }
}
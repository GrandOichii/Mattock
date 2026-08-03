using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Divination
/// </summary>
public class DivinationTests
{
    private static void HandCardCount(int pIdx, int expectedCount, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .AssertHand(ah => ah
                    .HasCardCount(expectedCount)
                )
            )
        );
    }

    [Fact]
    public async Task Baseline()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Divination");
        
        // Player A goes first
        // Player A skips to precombat main
        // Player A adds {2}{U} to their mana pool
        // Player A casts Divination
        // Player A resolves Divination
        // Player A crashes match, asserts that drew 2 cards

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ new() {
                Amount = 60,
                Card = card,
            } ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Blue, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 6, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HandCardCount(0, 8, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass();

        var match = new TestMatchWrapper(
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
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(8))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
            )
        );
    }
}
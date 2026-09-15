using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Mind Rot
/// </summary>
public class MindRotTests
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
    public async Task TargetSelf()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mind Rot");
        
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
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ChoosePlayers.Me()
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 6, a))
            .Act.AutoPassUntilStackEmpty()
            .ChooseCards.FirstN(2)
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
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(4))
                .AssertGraveyard(ag => ag.HasCardCount(3))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .AssertGraveyard(ag => ag.HasCardCount(0))
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task TargetOpponent()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mind Rot");
        
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
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ChoosePlayers.WithIdx(1)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 6, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            .ChooseCards.FirstN(2)
        ;

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
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
                .AssertGraveyard(ag => ag.HasCardCount(1))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(5))
                .AssertGraveyard(ag => ag.HasCardCount(2))
                .HasLife(20)
            )
        );
    }

    // [Fact]
    // public async Task TargetOpponent()
    // {
    //     // Arrange
    //     var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

    //     var card = loader.Load("M10:Mind Rot");
        
    //     var config = new MatchConfigBuilder()
    //         .FirstPlayerIdx(0)
    //         .NoManaPoolEmptying()
    //         .Build();
        
    //     var deck = new DeckTemplate()
    //     {
    //         MainDeck = [ new() {
    //             Amount = 60,
    //             Card = card,
    //         } ]
    //     };

    //     var p1 = new TestPlayerControllerBuilder("p1", 0)
    //         .SetDeck(deck)
    //         .ChoosePlayers.WithIdx(0)
    //         .Act.AddMana(ManaType.Black, 2)
    //         .Act.AutoPassToPhase(PhaseType.PrecombatMain)
    //         .Act.Assert(a => HandCardCount(0, 7, a))
    //         .Act.Assert(a => HasLife(0, 20, a))
    //         .Act.Assert(a => HandCardCount(1, 7, a))
    //         .Act.Assert(a => HasLife(1, 20, a))
    //         .Act.CastSpellWithName(card.Name)
    //         .ChoosePlayers.WithIdx(1)
    //         .ManaPaymentChoices.NTimes(2, smc => smc.First())
    //         .Act.AutoPassUntilStackEmpty()
    //         .Act.Assert(a => HandCardCount(0, 6, a))
    //         .Act.Assert(a => HasLife(0, 20, a))
    //         .Act.Assert(a => HandCardCount(1, 9, a))
    //         .Act.Assert(a => HasLife(1, 18, a))
    //         .Act.Crash()
    //     ;

    //     var p2 = new TestPlayerControllerBuilder("p2", 1)
    //         .SetDeck(deck)
    //         .Act.AutoPass();

    //     var match = new TestSessionWrapper(
    //         config,
    //         [ p1, p2 ]
    //     );
    //     match.RemoveMulligans();

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.Assert(a => a
    //         .CrashedIntentially()
    //         .NoChoicesLeft()
    //         .AssertPlayer(0, ap => ap
    //             .AssertHand(ah => ah.HasCardCount(6))
    //             .HasLife(20)
    //         )
    //         .AssertPlayer(1, ap => ap
    //             .AssertHand(ah => ah.HasCardCount(9))
    //             .HasLife(18)
    //         )
    //     );
    // }
}
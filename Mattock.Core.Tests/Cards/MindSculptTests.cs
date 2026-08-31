using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Mind Sculpt
/// </summary>
public class MindSculptTests
{
    private static void LibraryGraveyardSizes(int pIdx, int library, int graveyard, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .AssertLibrary(al => al.HasCardCount(library))
                .AssertGraveyard(al => al.HasCardCount(graveyard))
            )
        );
    }

    [Fact]
    public async Task TargetSingleOpponent()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Mind Sculpt");
        
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
            .Act.AddMana(ManaType.Blue, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => LibraryGraveyardSizes(0, 53, 0, a))
            .Act.Assert(a => LibraryGraveyardSizes(1, 53, 0, a))
            .Act.CastSpellWithName(card.Name)
            .ChoosePlayers.Assert(acp => acp.OptionsCount(1))
            .ChoosePlayers.WithIdx(1)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => LibraryGraveyardSizes(0, 53, 1, a))
            .Act.Assert(a => LibraryGraveyardSizes(1, 53 - 7, 7, a))
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
    public async Task RollbackBeforeTargetChoice()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Mind Sculpt");
        
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
            .Act.AddMana(ManaType.Blue, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => LibraryGraveyardSizes(0, 53, 0, a))
            .Act.Assert(a => LibraryGraveyardSizes(1, 53, 0, a))
            .Act.CastSpellWithName(card.Name)
            .ChoosePlayers.Assert(acp => acp.OptionsCount(1))
            .ChoosePlayers.Rollback.ToLast()
            .Act.Assert(a => LibraryGraveyardSizes(0, 53, 0, a))
            .Act.Assert(a => LibraryGraveyardSizes(1, 53, 0, a))
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
                .AssertHand(ah => ah.HasCardCount(7))
            )
            .CurrentPhase(PhaseType.PrecombatMain)
            .CurrentStep(null)
        );
    }
}
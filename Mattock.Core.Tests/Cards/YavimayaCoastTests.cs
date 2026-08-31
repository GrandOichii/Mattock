using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Yavimaya Coast
/// </summary>
public class YavimayaCoastTests
{
    private static void ManaPoolEmpty(int pIdx, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .AssertManaPool(amp => amp
                    .IsEmpty()
                )
            )
        );
    }

    private static void HasLife(int pIdx, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .HasLife(expected)
            )
        );
    }

    [Fact]
    public async Task ActivateGeneric()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Yavimaya Coast");
        
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
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a.CanPlayLand())
            .Act.PlayLandWithName(card.Name)
            .Act.Assert(a => a.CanActivateMana())
            .Act.Assert(a => ManaPoolEmpty(0, a))
            .Act.Assert(a => ManaPoolEmpty(1, a))
            .Act.ActivateMana(card.Name)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Assert(a => ManaPoolEmpty(1, a))
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
                .AssertManaPool(amp => amp
                    .HasTotalMana(1)
                    .HasMana(ManaType.Colorless, 1)
                )
            )
        );
    }

    [Theory]
    [InlineData("{G}", ManaType.Green)]
    [InlineData("{U}", ManaType.Blue)]
    public async Task ActivateColored(string choice, ManaType manaType)
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Yavimaya Coast");
        
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
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a.CanPlayLand())
            .Act.PlayLandWithName(card.Name)
            .Act.Assert(a => a.CanActivateMana())
            .Act.Assert(a => ManaPoolEmpty(0, a))
            .Act.Assert(a => ManaPoolEmpty(1, a))
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.ActivateMana(card.Name, 1)
            .StringChoices.Choose(choice)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Assert(a => ManaPoolEmpty(1, a))
            .Act.Assert(a => HasLife(1, 20, a))
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
                .HasLife(19)
                .AssertManaPool(amp => amp
                    .HasTotalMana(1)
                    .HasMana(manaType, 1)
                )
            )
        );
    }

    [Fact]
    public async Task RollbackBeforeChoice()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Yavimaya Coast");
        
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
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a.CanPlayLand())
            .Act.PlayLandWithName(card.Name)
            .Act.Assert(a => a.CanActivateMana())
            .Act.Assert(a => ManaPoolEmpty(0, a))
            .Act.Assert(a => ManaPoolEmpty(1, a))
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.ActivateMana(card.Name, 1)
            .StringChoices.Rollback.ToLast()
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
            .CurrentPhase(PhaseType.PrecombatMain)
            .AssertPlayer(0, ap => ap
                .HasLife(20)
                .AssertManaPool(amp => amp
                    .IsEmpty()
                )
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
                .AssertManaPool(amp => amp
                    .IsEmpty()
                )
            )
        );
    }
}
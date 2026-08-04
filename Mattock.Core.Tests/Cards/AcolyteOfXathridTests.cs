using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Acolyte of Xathrid
/// </summary>
public class AcolyteOfXathridTests
{
    private static void HasLife(int pIdx, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .HasLife(expected)
            )
        );
    }

    // [Fact]
    // public async Task CantActivate_NoMana()
    // {
    //     // Arrange
    //     var loader = new FileCardLoader("../../../../cards");

    //     var card = loader.Load("M10:Acolyte of Xathrid");
        
    //     var config = new MatchConfigBuilder()
    //         .FirstPlayerIdx(0)
    //         .NoManaPoolEmptying()
    //         .NoSummoningSickness()
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
    //         .Act.CastSpellWithName(card.Name)
    //         .ManaPaymentChoices.First()
    //         .Act.AutoPassUntilStackEmpty()
    //         .Act.Assert(a => a
    //             .CantActivate()
    //         )
    //         .Act.Crash()
    //     ;

    //     var p2 = new TestPlayerControllerBuilder("p2", 1)
    //         .SetDeck(deck)
    //         .Act.AutoPass();

    //     var match = new TestMatchWrapper(
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
    //             .HasLife(20)
    //         )
    //         .AssertPlayer(1, ap => ap
    //             .HasLife(20)
    //         )
    //     );
    // }

    [Fact]
    public async Task CantActivate_AlreadyTapped()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
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
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.First()
            .Act.AutoPassUntilStackEmpty()
            .Act.Tap(card.Name)
            .Act.AutoPassToPhase(PhaseType.PostcombatMain)
            .Act.Assert(a => a
                .CantActivate()
            )
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task CantActivate_SummoningSick()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
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
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.First()
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CantActivate()
            )
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task Activate_OnSelf()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
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
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.First()
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CanActivate()
            )
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.Activate(card.Name)
            .ChoosePlayers.Me()
            .PayMana.NTimes(2, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 19, a))
            .Act.Assert(a => HasLife(1, 20, a))
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
                .HasLife(19)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task Activate_OnOpp()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
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
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.First()
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CanActivate()
            )
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.Activate(card.Name)
            .ChoosePlayers.WithIdx(1)
            .PayMana.NTimes(2, smc => smc.First())
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasLife(0, 20, a))
            .Act.Assert(a => HasLife(1, 19, a))
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task PlayAndActivate_PayUsingManaAbilities()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var creature = loader.Load("M10:Acolyte of Xathrid");
        var swamp = loader.Load("M10:Swamp");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxLandsPerTurn()
            .NoSummoningSickness()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = creature,
                },
                new() {
                    Amount = 4,
                    Card = swamp,
                },

            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.PlayLandWithName(swamp.Name)
            .Act.PlayLandWithName(swamp.Name)
            .Act.PlayLandWithName(swamp.Name)
            .Act.CastSpellWithName(creature.Name)
            .ManaPaymentChoices.Assert(a => a
                .OptionsCount(4)
            )
            .ManaPaymentChoices.ActivateFirst()
            .ManaPaymentChoices.FirstStored()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertPlayer(0, ap => ap
                        .AssertManaPool(amp => amp
                            .HasTotalMana(1)
                        )
                    )
                )
            )
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CanActivate()
            )
            .Act.Activate(creature.Name)
            .ChoosePlayers.WithIdx(1)
            .ManaPaymentChoices.Assert(a => a
                .OptionsCount(3)
            )
            .ManaPaymentChoices.ActivateFirst()
            .ManaPaymentChoices.ActivateFirst()
            .ManaPaymentChoices.Assert(a => a
                .OptionsCount(3)
            )
            .PayMana.NTimes(2, smc => smc.FirstStored())
            .Act.AutoPassUntilStackEmpty()
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task CantActivate_NotFromBattlefield()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
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
            .Act.Assert(a => a
                .CantActivate()
            )
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task CantActivate_OppAbility()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Acolyte of Xathrid");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
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
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.First()
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a
                .CanActivate()
            )
            .Act.AutoPass()
            .DeclareAttack.Done()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.Black, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .CantActivate()
            )
            .Act.Crash()
        ;

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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }
}
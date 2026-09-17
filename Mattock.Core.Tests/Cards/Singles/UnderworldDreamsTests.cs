// UnderworldDreamsTests.cs

using Mattock.Core.Loaders;
using Mattock.Core.Matches.Turns.Steps.Ending;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Underworld Dreams
/// </summary>
public class UnderworldDreamsTests
{
    // private static void HandCardCount(int pIdx, int expectedCount, CommandChoicesBuilder.Asserts a)
    // {
    //     a.AssertMatch(am => am
    //         .AssertPlayer(pIdx, ap => ap
    //             .AssertHand(ah => ah
    //                 .HasCardCount(expectedCount)
    //             )
    //         )
    //     );
    // }

    [Fact]
    public async Task NoTrigger_OwnerTurnStart()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Underworld Dreams");
        var drawSpell = loader.Load("M10:Divination");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = card,
                },
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = drawSpell,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToStep(StepType.Draw)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPass()
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(8))
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task NoTrigger_OwnerDrawSpell()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Underworld Dreams");
        var drawSpell = loader.Load("M10:Divination");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoMaxHandSize()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 5,
                    Card = card,
                },
                new() {
                    Amount = 5,
                    Card = drawSpell,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 3)
            .Act.AddMana(ManaType.Blue, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(drawSpell.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task CheckTriggerOnOpp_TurnStart()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Underworld Dreams");
        var drawSpell = loader.Load("M10:Divination");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = card,
                },
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = drawSpell,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToStep(StepType.Draw)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsTriggeredAbility(ata => ata
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.Pass()
            .Act.Crash()
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(8))
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task CheckTriggerOnOpp_CardDrawSpell()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Underworld Dreams");
        var drawSpell = loader.Load("M10:Divination");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = card,
                },
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = drawSpell,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AddMana(ManaType.Blue, 3)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToStep(StepType.Draw)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsTriggeredAbility(ata => ata
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(drawSpell.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(2)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsTriggeredAbility(ata => ata
                                .CardName(card.Name)
                            )
                        )
                        .AssertEffect(1, ae => ae
                            .HasController(0)
                            .AssertAsTriggeredAbility(ata => ata
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(9))
                .HasLife(17)
            )
        );
    }
}
using Mattock.Core.Loaders;
using Mattock.Core.Matches.Turns.Steps.Ending;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Megrim
/// </summary>
public class MegrimTests
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

        var card = loader.Load("M10:Megrim");
        var discardSpell = loader.Load("M10:Mind Rot");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 4,
                    Card = card,
                },
                new() {
                    Amount = 3,
                    Card = discardSpell,
                } 
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 6)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(discardSpell.Name)
            .ChoosePlayers.Me()
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 5, a))
            .Act.Pass()
            .ChooseCards.FirstN(2)
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
                .AssertHand(ah => ah.HasCardCount(3))
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
    public async Task TargetOpp()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Megrim");
        var discardSpell = loader.Load("M10:Mind Rot");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 4,
                    Card = card,
                },
                new() {
                    Amount = 3,
                    Card = discardSpell,
                } 
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 6)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(discardSpell.Name)
            .ChoosePlayers.WithIdx(1)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 5, a))
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
            .Act.Pass()
            .Act.Pass()
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
                .AssertHand(ah => ah.HasCardCount(5))
                .AssertGraveyard(ag => ag.HasCardCount(1))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(5))
                .AssertGraveyard(ag => ag.HasCardCount(2))
                .HasLife(16)
            )
        );
    }

    [Fact]
    public async Task CheckDiscardAtEndOfTurn()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Megrim");
        var discardSpell = loader.Load("M10:Mind Rot");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 60,
                    Card = card,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToStep(StepType.Cleanup)
            .Act.Pass()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToStep(StepType.Cleanup)
            .ChooseCards.First()
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
                .AssertGraveyard(ag => ag.HasCardCount(0))
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .AssertGraveyard(ag => ag.HasCardCount(1))
                .HasLife(18)
            )
        );
    }
}
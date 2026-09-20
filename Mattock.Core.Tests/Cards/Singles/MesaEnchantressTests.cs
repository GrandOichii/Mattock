using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Mesa Enchantress
/// </summary>
public class MesaEnchantressTests
{
    [Fact]
    public async Task NoTriggerOnNonEnchantment()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mesa Enchantress");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 5,
                    Card = card,
                },
                new DeckCardTemplateBuilder("a")
                    .Amount(5)
                    .ZeroCost()
                    .Artifact()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsSpell(ass => ass.CardName("a"))
                        )
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
                .AssertHand(ah => ah.HasCardCount(5))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
            )
        );
    }

    [Fact]
    public async Task NoTriggerOnOppEnchantment()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mesa Enchantress");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 5,
                    Card = card,
                },
                new DeckCardTemplateBuilder("e")
                    .Amount(5)
                    .ZeroCost()
                    .Enchantment()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("e")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(1)
                            .AssertAsSpell(ass => ass.CardName("e"))
                        )
                    )
                )
            )
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
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
            )
        );
    }

    [Fact]
    public async Task TriggerOnEnchantment_Accept()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mesa Enchantress");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 5,
                    Card = card,
                },
                new DeckCardTemplateBuilder("e")
                    .Amount(5)
                    .ZeroCost()
                    .Enchantment()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("e")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(2)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsSpell(ass => ass.CardName("e"))
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
            .ChooseString.Yes()
            .Act.Pass()
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
                .AssertHand(ah => ah.HasCardCount(6))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
            )
        );
    }

    [Fact]
    public async Task TriggerOnEnchantment_Decline()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Mesa Enchantress");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 5,
                    Card = card,
                },
                new DeckCardTemplateBuilder("e")
                    .Amount(5)
                    .ZeroCost()
                    .Enchantment()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("e")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(2)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsSpell(ass => ass.CardName("e"))
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
            .ChooseString.No()
            .Act.Pass()
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
                .AssertHand(ah => ah.HasCardCount(5))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
            )
        );
    }

    // TODO test with rollbacks
}
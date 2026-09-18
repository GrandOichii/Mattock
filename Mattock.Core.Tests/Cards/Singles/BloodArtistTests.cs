
using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Blood Artist
/// </summary>
public class BloodArtistTests
{
    [Fact]
    public async Task NoTriggerOnNonCreatureDestruction()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("AVR:Blood Artist");
        var destroy = loader.Load("M10:Naturalize");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new() {
                    Amount = 1,
                    Card = destroy,
                },
                new DeckCardTemplateBuilder("a")
                    .Artifact()
                    .ZeroCost()
                    .Amount(1)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 2)
            .Act.AddMana(ManaType.Green, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(destroy.Name)
            .ChoosePermanents.Assert(a => a.OptionsCount(1))
            .ChoosePermanents.First()
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Pass()
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
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task SelfDeathTrigger()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("AVR:Blood Artist");
        var murder = loader.Load("M13:Murder");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new() {
                    Amount = 1,
                    Card = murder,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(murder.Name)
            .ChoosePermanents.Assert(a => a.OptionsCount(1))
            .ChoosePermanents.First()
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .ChoosePlayers.Assert(ap => ap.OptionsCount(2))
            .ChoosePlayers.WithIdx(1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .AssertAsTriggeredAbility(ast0 => ast0
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.Pass()
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
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task SameControllerDeathTrigger()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("AVR:Blood Artist");
        var murder = loader.Load("M13:Murder");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new() {
                    Amount = 1,
                    Card = murder,
                },
                new DeckCardTemplateBuilder("c")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/1")
                    .ZeroCost()
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("c")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(murder.Name)
            .ChoosePermanents.Assert(a => a.OptionsCount(2))
            .ChoosePermanents.WithName("c")
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .ChoosePlayers.Assert(ap => ap.OptionsCount(2))
            .ChoosePlayers.WithIdx(1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .AssertAsTriggeredAbility(ast0 => ast0
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.Pass()
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
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(19)
            )
        );
    }

    [Fact]
    public async Task OppCreatureDeathTrigger()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("AVR:Blood Artist");
        var murder = loader.Load("M13:Murder");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new() {
                    Amount = 1,
                    Card = murder,
                },
                new DeckCardTemplateBuilder("c")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/1")
                    .ZeroCost()
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.Black, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(murder.Name)
            .ChoosePermanents.Assert(a => a.OptionsCount(2))
            .ChoosePermanents.WithName("c")
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .ChoosePlayers.Assert(ap => ap.OptionsCount(2))
            .ChoosePlayers.WithIdx(1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .AssertAsTriggeredAbility(ast0 => ast0
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.Pass()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c")
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
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(19)
            )
        );
    }

    // TODO add test that forces the player to order the triggers
}
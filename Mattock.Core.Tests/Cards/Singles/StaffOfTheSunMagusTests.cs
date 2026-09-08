// StaffOfTheSunMagus.cs

using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Staff of the Sun Magus
/// </summary>
public class StaffOfTheSunMagusTests
{
    [Fact]
    public async Task TriggerOnPlains()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Staff of the Sun Magus");
        var plains = loader.Load("M10:Plains");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 2,
                    Card = plains,
                },
                new DeckCardTemplateBuilder("white-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .White()
                    .Build(),
                new DeckCardTemplateBuilder("blue-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .Blue()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, mpc => mpc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.PlayLandWithName(plains.Name)
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task NoTriggerOnOppPlains()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Staff of the Sun Magus");
        var plains = loader.Load("M10:Plains");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 2,
                    Card = plains,
                },
                new DeckCardTemplateBuilder("white-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .White()
                    .Build(),
                new DeckCardTemplateBuilder("blue-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .Blue()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, mpc => mpc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.AutoPass()
        ;
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.PlayLandWithName(plains.Name)
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }
    
    [Fact]
    public async Task TriggerOnWhiteInstant()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Staff of the Sun Magus");
        var plains = loader.Load("M10:Plains");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 2,
                    Card = plains,
                },
                new DeckCardTemplateBuilder("white-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .White()
                    .Build(),
                new DeckCardTemplateBuilder("blue-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .Blue()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, mpc => mpc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("white-instant")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(2)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsSpell(ata => ata
                                .CardName("white-instant")
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .HasLife(21)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task NoTriggerOnBlueInstant()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Staff of the Sun Magus");
        var plains = loader.Load("M10:Plains");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 2,
                    Card = plains,
                },
                new DeckCardTemplateBuilder("white-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .White()
                    .Build(),
                new DeckCardTemplateBuilder("blue-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .Blue()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, mpc => mpc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("blue-instant")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsSpell(ata => ata
                                .CardName("blue-instant")
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
            .AssertStack(ast => ast.IsEmpty())
            .AssertPlayer(0, ap => ap
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

    [Fact]
    public async Task NoTriggerOnOppWhiteInstant()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M15:Staff of the Sun Magus");
        var plains = loader.Load("M10:Plains");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 2,
                    Card = plains,
                },
                new DeckCardTemplateBuilder("white-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .White()
                    .Build(),
                new DeckCardTemplateBuilder("blue-instant")
                    .ZeroCost()
                    .Instant()
                    .Amount(1)
                    .Blue()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(3, mpc => mpc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.AutoPass()
        ;
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.CastSpellWithName("white-instant")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(1)
                            .AssertAsSpell(ata => ata
                                .CardName("white-instant")
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
        );
    }

}
using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Lightning Bolt
/// </summary>
public class LightningBoltTests
{
    [Fact]
    public async Task CheckTargetsAndTargetSelf()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Lightning Bolt");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c0")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a0")
                    .Amount(5)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new DeckCardTemplateBuilder("c1")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a1")
                    .Amount(6)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c0")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a0")
            .Act.AutoPassUntilStackEmpty()
            .Act.AddMana(ManaType.Red, 1)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(1, smc => smc.First())
            .AnyTargetsChoices.Assert(atca => atca
                .OptionsCount(4)
                .CanTargetPlayer(0)
                .CanTargetPlayer(1)
                .CanTargetPermanent("c0")
                .CanTargetPermanent("c1")
            )
            .AnyTargetsChoices.Me()
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a1")
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
                .HasLife(17)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(20)
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(4)
                .AssertPermanent("c0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("c1", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a1", ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }

    [Fact]
    public async Task TargetOpponent()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Lightning Bolt");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c0")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a0")
                    .Amount(5)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new DeckCardTemplateBuilder("c1")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a1")
                    .Amount(6)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c0")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a0")
            .Act.AutoPassUntilStackEmpty()
            .Act.AddMana(ManaType.Red, 1)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(1, smc => smc.First())
            .AnyTargetsChoices.PlayerWithIdx(1)
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a1")
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
                .HasLife(17)
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(4)
                .AssertPermanent("c0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("c1", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a1", ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }

    [Fact]
    public async Task TargetControlledCreature()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Lightning Bolt");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c0")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a0")
                    .Amount(5)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new DeckCardTemplateBuilder("c1")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a1")
                    .Amount(6)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c0")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a0")
            .Act.AutoPassUntilStackEmpty()
            .Act.AddMana(ManaType.Red, 1)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(1, smc => smc.First())
            .AnyTargetsChoices.PermanentWithName("c0")
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a1")
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
            .AssertBattlefield(ab => ab
                .HasPermanents(4)
                .AssertPermanent("c0", ap => ap
                    .HasMarkedDamage(3)
                )
                .AssertPermanent("c1", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a1", ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }

    [Fact]
    public async Task TargetOpponentsCreature()
    {
        // Arrange
        var loader = new FileCardLoader(new LuaCardScriptLoader("../../../../cards"), "../../../../cards");

        var card = loader.Load("M10:Lightning Bolt");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c0")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a0")
                    .Amount(5)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ 
                new DeckCardTemplateBuilder("c1")
                    .Amount(1)
                    .Creature()
                    .StatLine("0/4")
                    .ZeroCost()
                    .Build(),
                new DeckCardTemplateBuilder("a1")
                    .Amount(6)
                    .Artifact()
                    .ZeroCost()
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c0")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a0")
            .Act.AutoPassUntilStackEmpty()
            .Act.AddMana(ManaType.Red, 1)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(1, smc => smc.First())
            .AnyTargetsChoices.PermanentWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a1")
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
            .AssertBattlefield(ab => ab
                .HasPermanents(4)
                .AssertPermanent("c0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("c1", ap => ap
                    .HasMarkedDamage(3)
                )
                .AssertPermanent("a0", ap => ap
                    .HasNoMarkedDamage()
                )
                .AssertPermanent("a1", ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }
}
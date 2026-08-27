using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for checking that combat damage is dealt
/// </summary>
public class CombatDamageTests
{
    private static void HasLife(int pIdx, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .HasLife(expected)
            )
        );
    }

    private static void HasMarkedDamage(string name, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertBattlefield(ab => ab
                .AssertPermanent(name, ap => ap
                    .HasMarkedDamage(expected)
                )
            )
        );
    }

    private static void HasNoMarkedDamage(string name, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertBattlefield(ab => ab
                .AssertPermanent(name, ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }
    
    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 19)]
    [InlineData(5, 15)]
    [InlineData(10, 10)]
    public async Task DealtToPlayer_NoBlocker(int power, int expectedLife)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoSummoningSickness()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Power(power.ToString())
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ attacker ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("attacker")
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack
                .Player(attacker.Card.Name, 1)
                .Done()
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .Act.Assert(a => HasLife(1, 20, a))
            .Act.AutoPassToStep(StepType.CombatDamage)
            .Act.Assert(a => HasLife(1, expectedLife, a))
            .Act.AutoPassToPhase(PhaseType.PostcombatMain)
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(expectedLife)
            )
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker.Card.Name, ap => ap
                    .IsTapped()
                )
            )
        );
    }

    [Fact]
    public async Task AttackerAndBlocker_ExchangeDamage()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(1)
            .NoSummoningSickness()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .StatLine("3/5")
            .Build();

        var blocker = new DeckCardTemplateBuilder("blocker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .StatLine("4/4")
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ attacker ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ blocker ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(attacker.Card.Name)
            .Act.AutoPassToStep(StepType.BeginningOfCombat)
            .Act.Assert(a => HasNoMarkedDamage(attacker.Card.Name, a))
            .Act.Assert(a => HasNoMarkedDamage(blocker.Card.Name, a))
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack
                .Player(attacker.Card.Name, 1)
                .Done()
            .Act.Assert(a => HasNoMarkedDamage(attacker.Card.Name, a))
            .Act.Assert(a => HasNoMarkedDamage(blocker.Card.Name, a))
            .Act.AutoPassToStep(StepType.CombatDamage)
            .Act.Assert(a => HasMarkedDamage(attacker.Card.Name, 4, a))
            .Act.Assert(a => HasMarkedDamage(blocker.Card.Name, 3, a))
            .Act.AutoPassToPhase(PhaseType.PostcombatMain)
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(blocker.Card.Name)
            .Act.AutoPass()
            .DeclareAttack.Done()
            .DeclareBlock
                .Block(attacker.Card.Name, blocker.Card.Name)
                .Done()
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
            .AssertPlayer(0, ap => ap.HasLife(20))
            .AssertPlayer(1, ap => ap.HasLife(20))
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker.Card.Name, ap => ap.HasMarkedDamage(4))
                .AssertPermanent(blocker.Card.Name, ap => ap.HasMarkedDamage(3))
            )
            // TODO
        );
    }
}
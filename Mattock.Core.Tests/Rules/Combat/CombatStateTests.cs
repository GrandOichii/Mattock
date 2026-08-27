using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for checking that all combat steps are followed and the state of the attackers and blockers are correct
/// </summary>
public class CombatStateTests
{
    [Fact]
    public async Task NoAttacks_DeclareBlockersAndCombatDamageStepsAreSkipped()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
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
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToStep(StepType.BeginningOfCombat)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
            )
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
            )
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack.Skip()
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
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
            .TurnNumber(3)
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    private static void IsNotAttacking(int idx, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(ma => ma
            .AssertBattlefield(ab => ab
                .AssertPermanent(idx, ap => ap
                    .IsNotAttacking()
                )
            )
        );
    }

    private static void IsAttackingPlayer(int idx, int pIdx, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(ma => ma
            .AssertBattlefield(ab => ab
                .AssertPermanent(idx, ap => ap
                    .IsAttackingPlayer(pIdx)
                )
            )
        );
    }

    private static void IsBlocking(string blockerName, string attackerName, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(ma => ma
            .AssertBattlefield(ab => ab
                .AssertPermanent(blockerName, ap => ap
                    .IsBlocking(attackerName)
                )
            )
        );
    }

    private static void IsNotBlocking(string blockerName, string attackerName, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(ma => ma
            .AssertBattlefield(ab => ab
                .AssertPermanent(blockerName, ap => ap
                    .IsNotBlocking(attackerName)
                )
            )
        );
    }

    // [Fact]
    // public async Task AttackPlayer_NoBlockers_NoStepsAreSkipped()
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .FirstPlayerIdx(0)
    //         .GameLossIfRequiredToDrawFromEmptyLibrary(false)
    //         .Build();

    //     var attacker = new DeckCardTemplateBuilder("attacker")
    //         .ZeroCost()
    //         .AddType(CardTypes.Creature)
    //         .Build();
        
    //     var deck1 = new DeckTemplate()
    //     {
    //         MainDeck = [ attacker ]
    //     };

    //     var p1 = new TestPlayerControllerBuilder("p1", 0)
    //         .SetDeck(deck1)
    //         .ChoosePlayer.WithIdx(0)
    //         .Act.AutoPassToPhase(PhaseType.PrecombatMain)
    //         .Act.CastSpellWithName("attacker")
    //     ;
    // }

    [Fact]
    public async Task AttackPlayer_NoBlockers_NoStepsAreSkipped()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Power("0")
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
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToStep(StepType.BeginningOfCombat)
            // beginning of combat
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                    
                )
            )
            .Act.Assert(a => IsNotAttacking(0, a))
            .Act.Pass()
            // declare attackers
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack
                .Player("attacker", 1)
                .Done()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
            )
            .Act.Assert(a => IsAttackingPlayer(0, 1, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareBlockers)
                )
            )
            .Act.Assert(a => IsAttackingPlayer(0, 1, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.CombatDamage)
                )
            )
            .Act.Assert(a => IsAttackingPlayer(0, 1, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
            )
            .Act.Assert(a => IsAttackingPlayer(0, 1, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
            )
            .Act.Assert(a => IsNotAttacking(0, a))
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
            .TurnNumber(3)
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsTapped()
                )
            )
        );
    }

    [Fact]
    public async Task AttackPlayer_SingleBlocker()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var blocker = new DeckCardTemplateBuilder("blocker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .Power("0")
            .AddType(CardTypes.Creature)
            .Build();

        var nonattacker = new DeckCardTemplateBuilder("nonattacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ blocker ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ attacker, nonattacker ]
        };

         var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("blocker")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Skip()
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .DeclareBlock.Assert(ab => ab
                .OptionsCount(1)
                .CanBlock(attacker.Card.Name, blocker.Card.Name)
            )
            .DeclareBlock
                .Block(attacker.Card.Name, blocker.Card.Name)
                .Done()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareBlockers)
                )
            )
            .Act.Assert(a => IsBlocking(blocker.Card.Name, attacker.Card.Name, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.CombatDamage)
                )
            )
            .Act.Assert(a => IsBlocking(blocker.Card.Name, attacker.Card.Name, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
            )
            .Act.Assert(a => IsBlocking(blocker.Card.Name, attacker.Card.Name, a))
            .Act.Pass()
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
            )
            .Act.Assert(a => IsNotBlocking(blocker.Card.Name, attacker.Card.Name, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("attacker")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("nonattacker")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack
                .Player("attacker", 0)
                .Done()
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
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker.Card.Name, ap => ap
                    .IsTapped()
                    .IsNotAttacking()
                )
                .AssertPermanent(blocker.Card.Name, ap => ap
                    .IsUntapped()
                    .IsNotBlocking()
                )
            )
        );
    }
}
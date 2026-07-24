namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for checking that all combat steps are followed and the state of the attackers and defenders are correct
/// </summary>
public class AttackTests
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
            .ChoosePlayer.WithIdx(0)
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

        var match = new TestMatchWrapper(
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
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ attacker ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
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

        var match = new TestMatchWrapper(
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
}
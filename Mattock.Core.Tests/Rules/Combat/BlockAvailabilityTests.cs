namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for checking when and what can creatures block
/// </summary>
public class BlockAvailabilityTests
{
    [Fact]
    public async Task CanBlock_AttackingOnly()
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
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("blocker")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Skip()
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .DeclareBlock.Assert(ab => ab
                .OptionsCount(1)
                .CanBlock("attacker", "blocker")
            )
            .DeclareBlock
                .Done()
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
            .AssertBattlefield(ab => ab
                .AssertPermanent("attacker", ap => ap
                    .IsTapped()
                )
                .AssertPermanent("nonattacker", ap => ap
                    .IsUntapped()
                )
                .AssertPermanent("blocker", ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    /// <summary>
    /// Two blockers with baseline block count = 2 should be able to both block 2 attacking creatures
    /// </summary>
    [Fact]
    public async Task CanBlock_BothAttacking_BaselineBlockCount2()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .BaselineBlockCount(2)
            .Build();

        var blocker1 = new DeckCardTemplateBuilder("blocker1")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var blocker2 = new DeckCardTemplateBuilder("blocker2")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var attacker1 = new DeckCardTemplateBuilder("attacker1")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var attacker2 = new DeckCardTemplateBuilder("attacker2")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ blocker1, blocker2 ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ attacker1, attacker2 ]
        };

         var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(blocker1.Card.Name)
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(blocker2.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Skip()
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .DeclareBlock.Assert(ab => ab
                .OptionsCount(2)
                .CanBlock(attacker1.Card.Name, blocker1.Card.Name)
                .CanBlock(attacker2.Card.Name, blocker1.Card.Name)
                .CanBlock(attacker1.Card.Name, blocker2.Card.Name)
                .CanBlock(attacker2.Card.Name, blocker2.Card.Name)
            )
            .DeclareBlock
                .Done()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(attacker1.Card.Name)
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(attacker2.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack
                .Player(attacker1.Card.Name, 0)
                .Player(attacker2.Card.Name, 0)
                .Done()
            .Act.AutoPass()
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
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker1.Card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent(attacker2.Card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent(blocker1.Card.Name, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    [Fact]
    public async Task CantBlock_Tapped()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var untappedBlocker = new DeckCardTemplateBuilder("untappedBlocker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var tappedBlocker = new DeckCardTemplateBuilder("tappedBlocker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var deck1 = new DeckTemplate()
        {
            MainDeck = [ untappedBlocker, tappedBlocker ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ attacker ]
        };

         var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(untappedBlocker.Card.Name)
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(tappedBlocker.Card.Name)
            .Act.AutoPassUntilStackEmpty()
            .Act.Tap(tappedBlocker.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Skip()
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .DeclareBlock.Assert(ab => ab
                .OptionsCount(1)
                .CanBlock(attacker.Card.Name, untappedBlocker.Card.Name)
            )
            .DeclareBlock
                .Done()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(attacker.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack
                .Player(attacker.Card.Name, 0)
                .Done()
            .Act.AutoPass()
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
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker.Card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent(tappedBlocker.Card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent(untappedBlocker.Card.Name, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    public static IEnumerable<TheoryDataRow<string>> CantBlock_NonCreature_Data => [ 
        new(CardTypes.Enchantment),
        new(CardTypes.Artifact),
        new(CardTypes.Planeswalker),
        // TODO Battle
    ]; 

    [Theory]
    [MemberData(nameof(CantBlock_NonCreature_Data))]
    public async Task CantBlock_NotACreature(string type)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var nonblocker = new DeckCardTemplateBuilder("nonblocker")
            .ZeroCost()
            .AddType(type)
            .Build();

        var attacker = new DeckCardTemplateBuilder("attacker")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();

        var deck1 = new DeckTemplate()
        {
            MainDeck = [ nonblocker ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [ attacker ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(nonblocker.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareBlockers)
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(attacker.Card.Name)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack
                .Player(attacker.Card.Name, 0)
                .Done()
            .Act.AutoPass()
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
            .AssertBattlefield(ab => ab
                .AssertPermanent(attacker.Card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent(nonblocker.Card.Name, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    // TODO add check for Creatures that are also Battles (they can't block)
}
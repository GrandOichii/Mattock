using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for checking when and what can creatures attack
/// </summary>
public class AttackAvailabilityTests
{
    public static IEnumerable<TheoryDataRow<string>> NonCreature_CantAttack_Data => [ 
        new(CardTypes.Enchantment),
        new(CardTypes.Artifact),
        new(CardTypes.Planeswalker),
        // TODO Battle
    ]; 

    [Theory]
    [MemberData(nameof(NonCreature_CantAttack_Data))]
    public async Task NonCreature_CantAttack(string type)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var card = new DeckCardTemplateBuilder("p")
            .ZeroCost()
            .AddType(type)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ card ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("p")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            // * no attack declarations should be available
            .Act.Pass()
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
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    [Fact]
    public async Task Creature_CantAttack_SummoningSick()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var card = new DeckCardTemplateBuilder("p")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ card ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("p")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            // * no attack declarations should be available
            .Act.Pass()
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
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    [Fact]
    public async Task Creature_CanAttack_Player_RemovedSummoningSickness()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoSummoningSickness()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var card = new DeckCardTemplateBuilder("p")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ card ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("p")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack.Skip()
            .Act.Pass()
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
            .TurnNumber(1)
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    [Fact]
    public async Task Creature_CanAttack_Player_WaitOneTurn()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var card = new DeckCardTemplateBuilder("p")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ card ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("p")
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(1)
                .CanAttackPlayer(1)
            )
            .DeclareAttack.Skip()
            .Act.Pass()
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

    [Fact]
    public async Task TwoCreatures_CanAttack_Player()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoSummoningSickness()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();

        var card1 = new DeckCardTemplateBuilder("p1")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var card2 = new DeckCardTemplateBuilder("p2")
            .ZeroCost()
            .AddType(CardTypes.Creature)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ card1, card2 ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayer.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("p1")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("p2")
            .Act.AutoPassToStep(StepType.DeclareAttackers)
            .DeclareAttack.Assert(ada => ada
                .OptionsCount(2)
                .CanAttackPlayer("p1", 1)
                .CanAttackPlayer("p2", 1)
            )
            .DeclareAttack.Skip()
            .Act.Pass()
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
            .TurnNumber(1)
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertBattlefield(ab => ab
                .AssertPermanent(0, ap => ap
                    .IsUntapped()
                )
            )
        );
    }

    // TODO Creature_CanAttack_Planeswalker
    // TODO Creature_CanAttack_Battle
    // TODO check that if player chooses conflicting attacks that an exception is thrown
}
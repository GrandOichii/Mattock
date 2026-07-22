using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Tests.Rules.Combat;

/// <summary>
/// Tests for deciding when and what can creatures attack
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
    public async Task Creature_CanAttack_Player()
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


    // TODO Creature_CanAttack_Player


    // TODO Creature_CanAttack_Planeswalker
    // TODO Creature_CanAttack_Battle
}
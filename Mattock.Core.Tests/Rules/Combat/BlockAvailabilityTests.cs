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
                .CanBlock("attacker")
            )
            .DeclareBlock
                .Block("attacker", "defender")
                .Done()
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
            // TODO
        );
    }
}
using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Blinding Mage
/// </summary>
public class BlindingMageTests
{
    public static void CheckTapped(string name, bool expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertBattlefield(ab => ab
                .AssertPermanent(name, ap => ap
                    .CheckTapped(expected)
                )
            )
        );
    }
    [Fact]
    public async Task CantActivate_Tapped()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Blinding Mage");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoSummoningSickness()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ new() {
                Amount = 60,
                Card = card,
            } ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c")
                    .AddType(CardTypes.Creature)
                    .StatLine("0/1")
                    .ZeroCost()
                    .Amount(60)
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, s => s.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.Tap(card.Name)
            .Act.AutoPassToPhase(PhaseType.PostcombatMain)
            .Act.Assert(a => a.CantActivate())
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c")
            .AttackDeclarationsChoices.Done()
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
        );
    }

    [Fact]
    public async Task Activate_CheckTargets()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Blinding Mage");
        
        var config = new MatchConfigBuilder()
            .NoSummoningSickness()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ new() {
                Amount = 60,
                Card = card,
            } ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c")
                    .AddType(CardTypes.Creature)
                    .StatLine("0/1")
                    .ZeroCost()
                    .Amount(60)
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .ChoosePlayers.WithIdx(1)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, s => s.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => a.CanActivate())
            .Act.Assert(a => CheckTapped("c", false, a))
            .Act.Activate(card.Name)
            .PermanentsChoices.Assert(a => a
                .OptionsCount(2)
            )
            .PermanentsChoices.WithName("c")
            .ManaPaymentChoices.First()
            .Act.Assert(a => CheckTapped("c", false, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => CheckTapped("c", true, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c")
            .AttackDeclarationsChoices.Done()
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
                .AssertPermanent(card.Name, ap => ap
                    .IsTapped()
                )
                .AssertPermanent("c", ap => ap
                    .IsTapped()
                )
            )
        );
    }
}
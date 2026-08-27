using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Pyroclasm
/// </summary>
public class PyroclasmTests
{
    private static void HasDamage(string name, int expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertBattlefield(ab => ab
                .AssertPermanent(name, ap => ap
                    .HasMarkedDamage(expected)
                )
            )
        );
    }

    private static void HasNoDamage(string name, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertBattlefield(ab => ab
                .AssertPermanent(name, ap => ap
                    .HasNoMarkedDamage()
                )
            )
        );
    }

    [Fact]
    public async Task Baseline()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Pyroclasm");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(1)
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .NoManaPoolEmptying()
            .Build();
        
        var deck1 = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 6,
                    Card = card,
                }, 
                new DeckCardTemplateBuilder("c1")
                    .ZeroCost()
                    .AddType(CardTypes.Creature)
                    .StatLine("0/10")
                    .Build(),
            ]
        };

        var deck2 = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c2")
                    .ZeroCost()
                    .AddType(CardTypes.Creature)
                    .StatLine("0/10")
                    .Build(),
                new DeckCardTemplateBuilder("c3")
                    .ZeroCost()
                    .AddType(CardTypes.Creature)
                    .StatLine("0/10")
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck1)
            .Act.AutoPassToTurn(2)
            .Act.AddMana(ManaType.Red, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c1")
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasNoDamage("c1", a))
            .Act.Assert(a => HasNoDamage("c2", a))
            .Act.Assert(a => HasNoDamage("c3", a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.Assert(a => HasNoDamage("c1", a))
            .Act.Assert(a => HasNoDamage("c2", a))
            .Act.Assert(a => HasNoDamage("c3", a))
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => HasDamage("c1", 2, a))
            .Act.Assert(a => HasDamage("c2", 2, a))
            .Act.Assert(a => HasDamage("c3", 2, a))
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck2)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c2")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("c3")
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
                .AssertPermanent("c1", ap => ap
                    .HasMarkedDamage(2)
                )
                .AssertPermanent("c2", ap => ap
                    .HasMarkedDamage(2)
                )
                .AssertPermanent("c3", ap => ap
                    .HasMarkedDamage(2)
                )
            )
            .AssertPlayer(0, ap => ap.HasLife(20))
            .AssertPlayer(1, ap => ap.HasLife(20))
        );
    }
}
using KeraLua;
using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Black Cat
/// </summary>
public class BlackCatTests
{
    // TODO repeated code
    private static void HandCardCount(int pIdx, int expectedCount, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .AssertHand(ah => ah
                    .HasCardCount(expectedCount)
                )
            )
        );
    }

    [Fact]
    public async Task Baseline()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M15:Black Cat");
        var murder = loader.Load("M13:Murder");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ 
                new() {
                    Amount = 4,
                    Card = card,
                },
                new() {
                    Amount = 3,
                    Card = murder,
                },
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 5)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => HandCardCount(0, 7, a))
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(2, smc => smc.First())
            .Act.Assert(a => HandCardCount(0, 6, a))
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(murder.Name)
            .ChoosePermanents.Assert(a => a.OptionsCount(1))
            .ChoosePermanents.First()
            .ManaPaymentChoices.NTimes(3, smc => smc.First())
            .Act.Pass()
            .ChoosePlayers.Assert(ap => ap.OptionsCount(1))
            .ChoosePlayers.WithIdx(1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .AssertAsTriggeredAbility(ast0 => ast0
                                .CardName(card.Name)
                            )
                        )
                    )
                )
            )
            .Act.Pass()
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
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
                .AssertHand(ah => ah.HasCardCount(5))
                .AssertGraveyard(ag => ag.HasCardCount(2))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
                .AssertGraveyard(ah => ah.HasCardCount(1))
            )
        );
    }
}
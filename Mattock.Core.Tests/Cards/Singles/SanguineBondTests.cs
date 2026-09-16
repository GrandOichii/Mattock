using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Sanguine Bond
/// </summary>
public class SanguineBondTests
{
    [Fact]
    public async Task CheckTargetsAndTargetOpponent()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Sanguine Bond");
        var lifeGainCard = loader.Load("M10:Angel's Mercy");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false) 
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 4,
                    Card = lifeGainCard
                }
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Black, 5)
            .Act.AddMana(ManaType.White, 4)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName(lifeGainCard.Name)
            .ManaPaymentChoices.NTimes(4, smc => smc.First())
            .Act.Pass()
            .ChoosePlayers.Assert(a => a.OptionsCount(1))
            .ChoosePlayers.WithIdx(1)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .EffectCount(1)
                        .AssertEffect(0, ae => ae
                            .HasController(0)
                            .AssertAsTriggeredAbility(ata => ata
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
                .HasLife(27)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(13)
            )
        );
    }

    [Fact]
    public async Task NoTriggerOnOpp()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Sanguine Bond");
        var lifeGainCard = loader.Load("M10:Angel's Mercy");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false) 
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 3,
                    Card = card,
                },
                new() {
                    Amount = 4,
                    Card = lifeGainCard
                }
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .Act.AddMana(ManaType.Black, 5)
            .ChoosePlayers.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(5, smc => smc.First())
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AddMana(ManaType.White, 4)
            .Act.AutoPassToPhase(PhaseType.PostcombatMain)
            .Act.CastSpellWithName(lifeGainCard.Name)
            .ManaPaymentChoices.NTimes(4, smc => smc.First())
            .Act.AutoPassUntilStackEmpty()
            .Act.Crash()
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
                .HasLife(20)
            )
            .AssertPlayer(1, ap => ap
                .HasLife(27)
            )
            .AssertStack(ast => ast
                .EffectCount(0)
            )
        );
    }

}
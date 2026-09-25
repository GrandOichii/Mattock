using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Honor the Pure
/// </summary>
public class HonorThePureTests
{
    [Fact]
    public async Task NoEffectWhileInHand()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Honor the Pure");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c")
                    .White()
                    .Creature()
                    .StatLine("0/1")
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName("c")
            .Act.AutoPassUntilStackEmpty()
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
            .AssertBattlefield(ab => ab
                .HasPermanents(1)
                .AssertPermanent(0, ap => ap
                    .HasPower(0)
                    .HasToughness(1)
                )
            )
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah
                    .AssertCard(0, ac => ac
                        .HasPower(0)
                        .HasPower(1)
                    )
                )
            )
        );
    }

    [Fact]
    public async Task NoEffectForCreaturesInHand()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Honor the Pure");
        
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new() {
                    Amount = 1,
                    Card = card,
                },
                new DeckCardTemplateBuilder("c")
                    .White()
                    .Creature()
                    .StatLine("0/1")
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.White, 3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .Act.AutoPassUntilStackEmpty()
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
            .AssertBattlefield(ab => ab
                .HasPermanents(1)
                .AssertPermanent(0, ap => ap
                    .HasPower(0)
                    .HasToughness(1)
                )
            )
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah
                    .AssertCard(0, ac => ac
                        .HasPower(0)
                        .HasPower(1)
                    )
                )
            )
            // TODO assert continuous effects
        );
    }
}
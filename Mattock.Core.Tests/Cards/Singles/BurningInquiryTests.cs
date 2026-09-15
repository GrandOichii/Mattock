using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards.Singles;

/// <summary>
/// Tests for the card Burning Inquiry
/// </summary>
public class BurningInquiryTests
{
    [Fact]
    public async Task Baseline()
    {
        // Arrange
        var loader = new FileCardLoader(new JsonCardScriptLoader("../../../../Scripts"), "../../../../cards");

        var card = loader.Load("M10:Burning Inquiry");

        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [ new() {
                Amount = 60,
                Card = card,
            } ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.WithIdx(0)
            .Act.AddMana(ManaType.Red, 1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.CastSpellWithName(card.Name)
            .ManaPaymentChoices.NTimes(1, smc => smc.First())
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
            .AssertPlayer(0, ap => ap
                .AssertHand(ah => ah.HasCardCount(6))
                .AssertGraveyard(ah => ah.HasCardCount(4))
            )
            .AssertPlayer(1, ap => ap
                .AssertHand(ah => ah.HasCardCount(7))
                .AssertGraveyard(ah => ah.HasCardCount(3))
            )
        );
    }
}
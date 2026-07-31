using Mattock.Core.Loaders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Naturalize
/// </summary>
public class NaturalizeTests
{
    [Fact]
    public async Task NoTargets()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Naturalize");

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
            .ChoosePlayer.WithIdx(0)
            .Act.AddMana(ManaType.Green, 2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .CantCastSpell()
            )
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
        );
    }

    // TODO add actual tests
}
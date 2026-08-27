using Mattock.Core.Loaders;
using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Cards;

/// <summary>
/// Tests for the card Forest
/// </summary>
public class ForestTests
{
    private static void ManaPoolEmpty(int pIdx, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertPlayer(pIdx, ap => ap
                .AssertManaPool(amp => amp
                    .IsEmpty()
                )
            )
        );
    }

    [Fact]
    public async Task Baseline()
    {
        // Arrange
        var loader = new FileCardLoader("../../../../cards");

        var card = loader.Load("M10:Forest");
        
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
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a.CanPlayLand())
            .Act.PlayLandWithName("Forest")
            .Act.Assert(a => a.CanActivateMana())
            .Act.Assert(a => ManaPoolEmpty(0, a))
            .Act.Assert(a => ManaPoolEmpty(1, a))
            .Act.ActivateMana("Forest")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertStack(ast => ast
                        .IsEmpty()
                    )
                )
            )
            .Act.Assert(a => ManaPoolEmpty(1, a))
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
            .AssertPlayer(0, ap => ap
                .AssertManaPool(amp => amp
                    .HasTotalMana(1)
                    .HasColoredMana(ManaType.Green, 1)
                )
            )
            .NoChoicesLeft()
        );
    }
}
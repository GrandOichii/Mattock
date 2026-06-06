using System.Windows.Input;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.Zones;

public class GeneralZoneTests
{
    [Fact]
    [Trait("Rules", "400.3.")]
    public async Task OwnedZoneChangeCorrection_MillFromOppsDeck()
    {
        // Arrange
        var deckSize = 10;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Enqueue((
                (
                    async (match, player, options) =>
                    {
                        var opp = match.Match!.Players[1];
                        match.Match!.MoveCard(
                            opp.Library.GetLast()!,
                            player.Graveyard,
                            CardZoneChangeType.Bottom
                        );
                        return (null, false, true);
                    },
                    true
                )
            ))
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .InitialHandSize(0)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertPlayer(1, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize - 1)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(1)
                )
            )
        );
    }

    [Fact]
    [Trait("Rules", "400.3.")]
    public async Task OwnedZoneChangeCorrection_DrawFromOppsDeck()
    {
        // Arrange
        var deckSize = 10;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Enqueue((
                (
                    async (match, player, options) =>
                    {
                        var opp = match.Match!.Players[1];
                        match.Match!.MoveCard(
                            opp.Library.GetLast()!,
                            player.Hand,
                            CardZoneChangeType.Bottom
                        );
                        return (null, false, true);
                    },
                    true
                )
            ))
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .InitialHandSize(0)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertPlayer(1, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize - 1)
                )
                .AssertHand(ad => ad
                    .HasCardCount(1)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
        );
    }

    [Fact]
    [Trait("Rules", "400.3.")]
    public async Task OwnedZoneChangeCorrection_MoveToDeckFromOppsGraveyard()
    {
        // Arrange
        var deckSize = 10;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Enqueue((
                (
                    async (match, player, options) =>
                    {
                        var opp = match.Match!.Players[1];
                        match.Match!.MoveCard(
                            opp.Hand.GetLast()!,
                            player.Library,
                            CardZoneChangeType.Bottom
                        );
                        return (null, false, true);
                    },
                    true
                )
            ))
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .InitialHandSize(deckSize)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(deckSize)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertPlayer(1, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(1)
                )
                .AssertHand(ad => ad
                    .HasCardCount(deckSize - 1)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
        );
    }

    [Theory]
    [Trait("Rules", "400.4a")]
    [InlineData("Instant")]
    [InlineData("Sorcery")]
    public async Task MoveInstantOrSorceryToBattlefield(string type)
    {
        // Arrange
        var deckSize = 10;
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(deckSize)
                    .AddType(type)
                    .Build(),
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            // * upkeep 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Enqueue((
                (
                    async (match, player, options) =>
                    {
                        var opp = match.Match!.Players[1];
                        match.Match!.MoveCard(
                            opp.Library.GetLast()!,
                            match.Match!.Battlefield,
                            CardZoneChangeType.Bottom
                        );
                        return (null, false, true);
                    },
                    true
                )
            ))
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .InitialHandSize(0)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(deckSize)
                )
                .AssertHand(ad => ad
                    .HasCardCount(0)
                )
                .AssertGraveyard(ad => ad
                    .HasCardCount(0)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(0)
            )
        );
    }

}
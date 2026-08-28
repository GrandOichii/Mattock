using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Rollback;

public class SnapshotRollbackTests
{
    class ExpectedMatch
    {
        public required int TurnNumber { get; init; }
        public required int SnapshotCount { get; init; }
        public required int ActivePlayerIdx { get; init; }
    }

    private static void Expect(ExpectedMatch expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .TurnNumber(expected.TurnNumber)
            .AssertSnapshots(asn => asn.HasCount(expected.SnapshotCount))
            .ActivePlayerIs(expected.ActivePlayerIdx)
        );
    }

    [Fact]
    public async Task TODONameMe()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder()
                    .ZeroCost()
                    .Artifact()
                    .Amount(60)
                    .Build()
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => 
                Expect(new()
                {
                    SnapshotCount = 1,
                    TurnNumber = 1,
                    ActivePlayerIdx = 0,
                }, a)
                // .AssertMatch(am => am
                //     .AssertPlayer(0, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(7))
                //     )
                //     .AssertPlayer(1, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(7))
                //     )
                //     .AssertSnapshots(asn => asn.HasCount(1))
                // )

            )
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToTurn(1)
            // // post rollback
            .Act.Assert(a => 
                Expect(new()
                {
                    SnapshotCount = 1,
                    TurnNumber = 1,
                    ActivePlayerIdx = 0,
                }, a)
                // .AssertMatch(am => am
                //     .AssertPlayer(0, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(7))
                //     )
                //     .AssertPlayer(1, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(7))
                //     )
                //     .AssertSnapshots(asn => asn
                //         .HasCount(2)
                //         .AssertSnapshot(0, asn0 => asn0
                //             .HasId("turn-1")
                //         )
                //         .AssertSnapshot(1, asn1 => asn1
                //             .HasId("turn-2")
                //         )
                //     )
                // )
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => 
                Expect(new()
                {
                    SnapshotCount = 2,
                    TurnNumber = 2,
                    ActivePlayerIdx = 1,
                }, a)
                // .AssertMatch(am => am
                //     .AssertPlayer(0, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(7))
                //     )
                //     .AssertPlayer(1, ap => ap
                //         .AssertHand(ah => ah.HasCardCount(8))
                //     )
                //     .AssertSnapshots(asn => asn
                //         .HasCount(2)
                //         .AssertSnapshot(0, asn0 => asn0
                //             .HasId("turn-1")
                //         )
                //         .AssertSnapshot(1, asn1 => asn1
                //             .HasId("turn-2")
                //         )
                //     )
                // )
            )
            .Act.Rollback(1)
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
            // TODO
        );
    }
}
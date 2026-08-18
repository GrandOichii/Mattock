namespace Mattock.Core.Tests.Rollback;

public class SnapshotLoadingTests
{
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
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertPlayer(0, ap => ap
                        .AssertHand(ah => ah.HasCardCount(7))
                    )
                    .AssertPlayer(1, ap => ap
                        .AssertHand(ah => ah.HasCardCount(7))
                    )
                    .AssertSnapshots(asn => asn.HasCount(1))
                )
            )
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertPlayer(0, ap => ap
                        .AssertHand(ah => ah.HasCardCount(7))
                    )
                    .AssertPlayer(1, ap => ap
                        .AssertHand(ah => ah.HasCardCount(8))
                    )
                    .AssertSnapshots(asn => asn
                        .HasCount(2)
                        .AssertSnapshot(0, asn0 => asn0
                            .HasId("turn-1")
                        )
                        .AssertSnapshot(1, asn0 => asn0
                            .HasId("turn-2")
                        )
                    )
                )
            )
            .Act.Rollback("turn-1")
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .AssertPlayer(0, ap => ap
                        .AssertHand(ah => ah.HasCardCount(7))
                    )
                    .AssertPlayer(1, ap => ap
                        .AssertHand(ah => ah.HasCardCount(7))
                    )
                    .AssertSnapshots(asn => asn
                        .HasCount(2)
                        .AssertSnapshot(0, asn0 => asn0
                            .HasId("turn-1")
                        )
                        .AssertSnapshot(1, asn0 => asn0
                            .HasId("turn-2")
                        )
                    )
                )
            )
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
}
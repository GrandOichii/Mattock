using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Rollback;

public class SnapshotRollbackTests
{
    class ExpectedMatch
    {
        public required int TurnNumber { get; init; }
        public required int ActivePlayerIdx { get; init; }
        public required Player[] Players { get; init; }
        // public required Snapshot[] Snapshots { get; init; }
        public required StackEffect[] StackEffects { get; init; }
        public required PhaseType Phase { get; init; }
        public required StepType? Step { get; init; }

        public class StackEffect
        {
            public required int ControllerIdx { get; init; }
        }

        public class Player
        {
            public required int Life { get; init; }
            public required int HandCount { get; init; }
            public required int LibraryCount { get; init; }
            public required int GraveyardCount { get; init; }
            public required int PermanentCount { get; init; }

            public class Permanent
            {
                public required string Name { get; init; }
            }
        }

        // public class Snapshot
        // {
        //     public required int TurnNumber { get; init; }
        // }
    }

    private static void Expect(ExpectedMatch expected, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .TurnNumber(expected.TurnNumber)
            // .AssertSnapshots(asn => asn.HasCount(expected.Snapshots.Length))
            .ActivePlayerIs(expected.ActivePlayerIdx)
            .AssertStack(ast => ast.EffectCount(expected.StackEffects.Length))
            .CurrentPhase(expected.Phase)
            .CurrentStep(expected.Step)
        );

        for (int i = 0; i < expected.Players.Length; ++i)
        {
            var e = expected.Players[i];
            a.AssertMatch(am => am.AssertPlayer(i, ap => ap
                .HasLife(e.Life)
                .AssertLibrary(al => al
                    .HasCardCount(e.LibraryCount)
                )
                .AssertHand(al => al
                    .HasCardCount(e.HandCount)
                )
                .AssertGraveyard(al => al
                    .HasCardCount(e.GraveyardCount)
                )
                .ControlledPermanentsCount(e.PermanentCount)
            ));
        }

        for (int i = 0; i < expected.StackEffects.Length; ++i)
        {
            var e = expected.StackEffects[i];
            a.AssertMatch(am => am.AssertStack(ast => ast.AssertEffect(i, ae => ae
                .HasController(e.ControllerIdx)
            )));
        }

        // for (int i = 0; i < expected.Snapshots.Length; ++i)
        // {
        //     var e = expected.Snapshots[i];
        //     a.AssertMatch(am => am.AssertSnapshots(asn => asn.AssertSnapshot(i, asni => asni
        //         .TurnNumber(e.TurnNumber)
        //     )));
        // }
    }

    [Fact]
    public async Task SecondPlayerRollsBackToTurn1()
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

        ExpectedMatch turn1 = new()
        {
            TurnNumber = 1,
            ActivePlayerIdx = 0,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     }
            // ]
        };

        ExpectedMatch turn2 = new()
        {
            TurnNumber = 2,
            ActivePlayerIdx = 1,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 8,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     },
            //     new() {
            //         TurnNumber = 2,
            //     }
            // ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => 
                Expect(turn1, a)
            )
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToTurn(1)
            // * post rollback
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => 
                Expect(turn1, a)
            )
            .Act.Crash()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => 
                Expect(turn2, a)
            )
            .Act.Rollback.ToTurn(1)
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
            // TODO
        );
    }

    [Fact]
    public async Task FirstPlayerRollsBackToTurn2()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
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

        ExpectedMatch turn2 = new()
        {
            TurnNumber = 2,
            ActivePlayerIdx = 1,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 8,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     },
            //     new() {
            //         TurnNumber = 2,
            //     },
            // ]
        };

        ExpectedMatch turn3 = new()
        {
            TurnNumber = 3,
            ActivePlayerIdx = 0,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 8,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 8,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     },
            //     new() {
            //         TurnNumber = 2,
            //     },
            //     new() {
            //         TurnNumber = 3,
            //     },
            // ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(turn3, a))
            .Act.Rollback.ToTurn(2)
            .Act.AutoPass()
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(turn2, a))
            .Act.AutoPassToTurn(3)
            .Act.AutoPassToTurn(2)
            // * post rollback
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(turn2, a))
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
            // TODO
        );
    }

    [Fact]
    public async Task CheckLibraryOrder()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("a")
                    .ZeroCost()
                    .Artifact()
                    .Amount(60)
                    .Build()
            ]
        };

        List<string> libraryIds = [];

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            // TODO remember library order
            .Act.Enqueue((
                async (wrapper, player, options) =>
                {
                    foreach (var card in player.Library.Cards)
                        libraryIds.Add(card.Id);
                    return ((null, null), false, true);
                },
                true
            ))
            .Act.Rollback.ToTurn(1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Enqueue((
                async (wrapper, player, options) =>
                {
                    player.Library.Cards.Count.ShouldBe(libraryIds.Count);
                    for (int i = 0; i < libraryIds.Count; ++i)
                        player.Library.Cards[i].Id.ShouldBe(libraryIds[i]);
                    return ((null, null), false, true);
                },
                true
            ))
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
        );
    }

    [Fact]
    public async Task FirstPlayerCastsTwoArtifactsAndRollsBackToStartOfTurn()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("a")
                    .ZeroCost()
                    .Artifact()
                    .Amount(60)
                    .Build()
            ]
        };

        ExpectedMatch preCast = new()
        {
            TurnNumber = 1,
            ActivePlayerIdx = 0,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     }
            // ]
        };

        ExpectedMatch postCast = new()
        {
            TurnNumber = 1,
            ActivePlayerIdx = 0,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 5,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 2,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     }
            // ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
            .Act.CastSpellWithName("a")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a")
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => Expect(postCast, a))
            .Act.Rollback.ToTurn(1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
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
            // TODO
        );
    }

    [Fact]
    public async Task SecondPlayerCastsTwoArtifactsAndRollsBackToStartOfTurn()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("a")
                    .ZeroCost()
                    .Artifact()
                    .Amount(60)
                    .Build()
            ]
        };

        ExpectedMatch preCast = new()
        {
            TurnNumber = 2,
            ActivePlayerIdx = 1,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 8,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     },
            //     new() {
            //         TurnNumber = 2,
            //     }
            // ]
        };

        ExpectedMatch postCast = new()
        {
            TurnNumber = 2,
            ActivePlayerIdx = 1,
            StackEffects = [],
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 6,
                    LibraryCount = 52,
                    Life = 20,
                    PermanentCount = 2,
                }
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     },
            //     new() {
            //         TurnNumber = 2,
            //     }
            // ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPass()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
        ;

        var p2 = new TestPlayerControllerBuilder("p2", 1)
            .SetDeck(deck)
            .Act.AutoPassToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
            .Act.CastSpellWithName("a")
            .Act.AutoPassUntilStackEmpty()
            .Act.CastSpellWithName("a")
            .Act.AutoPassUntilStackEmpty()
            .Act.Assert(a => Expect(postCast, a))
            .Act.Rollback.ToTurn(2)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
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
            // TODO
        );
    }

    [Fact]
    public async Task FirstPlayerCastsTwoInstantsAndRollsBackToStartOfTurn()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .NoMaxHandSize()
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("i")
                    .ZeroCost()
                    .Instant()
                    .Amount(60)
                    .Build()
            ]
        };

        ExpectedMatch preCast = new()
        {
            TurnNumber = 1,
            ActivePlayerIdx = 0,
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            StackEffects = [
                
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     }
            // ]
        };

        ExpectedMatch postCast = new()
        {
            TurnNumber = 1,
            ActivePlayerIdx = 0,
            Phase = PhaseType.PrecombatMain,
            Step = null,
            Players = [
                new() {
                    GraveyardCount = 0,
                    HandCount = 5,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                },
                new() {
                    GraveyardCount = 0,
                    HandCount = 7,
                    LibraryCount = 53,
                    Life = 20,
                    PermanentCount = 0,
                }
            ],
            StackEffects = [
                new() {
                    ControllerIdx = 0,
                },
                new() {
                    ControllerIdx = 0,
                },
            ],
            // Snapshots = [
            //     new() {
            //         TurnNumber = 1,
            //     }
            // ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
            .Act.CastSpellWithName("i")
            .Act.CastSpellWithName("i")
            .Act.Assert(a => Expect(postCast, a))
            .Act.Rollback.ToTurn(1)
            .Act.AutoPassToPhase(PhaseType.PrecombatMain)
            .Act.Assert(a => Expect(preCast, a))
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
            // TODO
        );
    }
}
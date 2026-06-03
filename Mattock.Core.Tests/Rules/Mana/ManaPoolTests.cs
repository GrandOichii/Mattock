using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.Mana;

// TODO rename?
public class ManaPoolTests
{
    public static IEnumerable<object[]> AddGeneric_Data => [
        [
            new (ManaType, int)[] {
                (ManaType.White, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Blue, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Black, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Red, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Green, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Colorless, 1),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.White, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Blue, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Black, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Red, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Green, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.White, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Blue, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Black, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Red, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Green, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.Green, 10),
                (ManaType.Colorless, 10),
                (ManaType.Colorless, 10),
                (ManaType.Colorless, 10),
            }
        ],
        [
            new (ManaType, int)[] {
                (ManaType.White, 1),
                (ManaType.Blue, 1),
                (ManaType.Black, 1),
                (ManaType.Red, 1),
                (ManaType.Green, 1),
                (ManaType.Colorless, 1),
            }
        ],
    ];

    [Theory]
    [Trait("Rules", "500.5.")]
    [MemberData(nameof(AddGeneric_Data))]
    public async Task TotalMana((ManaType type, int amount)[] mana)
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var total = mana.Sum(m => m.amount);

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // *** turn 1
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .AssertPlayer(0, sa => sa
                        .AssertManaPool(pa => pa
                            .IsEmpty()
                        )
                    )
                )
            )
            .Act.ForEach(mana, (m, a) => a
                .AddMana(m.type, m.amount)
            )
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .AssertPlayer(0, sa => sa
                        .AssertManaPool(pa => pa
                            .HasTotalMana(total)
                        )
                    )
                )
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }

    [Theory]
    [Trait("Rules", "500.5.")]
    [MemberData(nameof(AddGeneric_Data))]
    public async Task EmptiesAfterEachPhaseAndStep((ManaType type, int amount)[] mana)
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        static void manaPoolIsEmpty(CommandChoicesBuilder.Asserts a) => a
            .AssertMatch(ma => ma
                .AssertPlayer(0, sa => sa
                    .AssertManaPool(pa => pa
                        .IsEmpty()
                    )
                )
            );

        var p1 = new TestPlayerControllerBuilder("p1")
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
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PrecombatMain)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
            )
            // TODO declare attackers
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.AddMana(ManaType.Colorless, 1)
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
            )
            .Act.Assert(manaPoolIsEmpty)
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .Act.AutoPass()
            ;

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .FirstPlayerNoDrawIfSingleOpponent(false)
                .Build(),
            [ p1, p2 ]
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );
    }
}
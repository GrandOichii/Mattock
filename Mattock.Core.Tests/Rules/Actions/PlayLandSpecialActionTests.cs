using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Rules.Actions;

public class PlayLandSpecialActionTests
{
    [Trait("Rules", "116.2a,305.1.,305.2.,305.3.")]
    [Fact]
    public async Task CheckLandPlayabilityInEachPhaseAndStep()
    {
        // Player B will auto-pass always
        // Player A will check on each step and phase that they can/can't play lands
        // Player A goes first
        // Player A passes to precombat main, plays land l1
        // Player A passes to their next turn
        // Player A passes to their postcombat main, plays land l2
        // Player A passes to their end step
        // Player A crashes

        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
                new DeckCardTemplateBuilder("c1")
                    .Amount(4)
                    .Build(),
                new DeckCardTemplateBuilder("l1")
                    .Amount(2)
                    .Land()
                    .Build(),
                new DeckCardTemplateBuilder("l2")
                    .Amount(1)
                    .Land()
                    .Build(),
            ]
        };

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
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * draw 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * precombat main phase 1
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanPlayLand()
                .OptionsCount(1 + 3) // pass + 3 available lands
            )
            .Act.PlayLandWithName("l1")
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * beginning of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * declare attackers 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * postcombat main 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * end 1
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // *** turn 2
            // * upkeep 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * draw 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * precombat main phase 2
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * beginning of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * declare attackers 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * end of combat 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * postcombat main 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * end 2
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // *** turn 3
            // * upkeep 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * draw 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * precombat main phase 3
            .Act.Assert(a => a
                .AssertMatch(am => am.CurrentPhase(PhaseType.PrecombatMain))
                .CanPass()
                .CanPlayLand()
                .OptionsCount(1 + 2) // pass + 2 available lands
            )
            .Act.Pass()
            // * beginning of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * declare attackers 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // TODO declare attackers
            // * end of combat 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Pass()
            // * postcombat main 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
                .CanPass()
                .CanPlayLand()
                .OptionsCount(1 + 2) // pass + 2 available lands
            )
            .Act.PlayLandWithName("l2")
            .Act.Pass()
            // * end 3
            .Act.Assert(a => a
                .AssertMatch(am => am
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
                .CanPass()
                .CantPlayLand()
            )
            .Act.Crash()
            ;

        var p2 = new TestPlayerControllerBuilder("p2")
            .SetDeck(deck)
            .Act.AutoPass()
            // TODO declare attackers
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
            .AssertPlayer(0, ap => ap
                .AssertLibrary(ad => ad
                    .HasCardCount(0)
                )
                .AssertHand(ad => ad
                    .HasCardCount(5)
                )
            )
            .AssertBattlefield(ab => ab
                .HasPermanents(2)
                .AssertPermanent("l1", ap => ap
                    .IsLand()
                    .ControlledBy(0)
                    .IsUntapped()
                )
                .AssertPermanent("l2", ap => ap
                    .IsLand()
                    .ControlledBy(0)
                    .IsUntapped()
                )
            )
        );
    }
}
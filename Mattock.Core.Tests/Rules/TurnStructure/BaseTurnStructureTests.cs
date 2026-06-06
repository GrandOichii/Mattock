using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps;

namespace Mattock.Core.Tests.Rules.TurnStructure;

public class BaseTurnStructureTests
{
    [Fact]
    [Trait("Rules", "103.8.,500.1.,501.,502.,503.,504.,505.,506.,507.,508.,509.,510.,511.,512.,513.,514.")]
    public async Task CheckPhasesAndSteps()
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = [
            ]
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            // * upkeep
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .MatchPhases(
                        PhaseType.Beginning,
                        PhaseType.PrecombatMain,
                        PhaseType.Combat,
                        PhaseType.PostcombatMain,
                        PhaseType.Ending
                    )
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Pass() // pass priority around
            // * draw
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
            )
            .Act.Pass() // pass priority around
            // * precombat main
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                )
            )
            .Act.Pass() // pass priority around
            // * beginning of combat phase
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
            )
            .Act.Pass() // pass priority around
            // * declare attackers
            // TODO choose attackers
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
            )
            .Act.Pass() // pass priority around
            // * end of combat
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
            )
            .Act.Pass() // pass priority around
            // * postcombat main
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
            )
            .Act.Pass() // pass priority around
            // * end
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
            )
            .Act.Pass() // pass priority around
            ;

        TestPlayerControllerBuilder notP1(string name, int teamIdx) => new TestPlayerControllerBuilder(name, teamIdx)
            // * upkeep
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Upkeep)
                )
            )
            .Act.Pass() // pass priority around
            // * draw
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Beginning)
                    .CurrentStep(StepType.Draw)
                )
            )
            .Act.Pass() // pass priority around
            // * precombat main
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PrecombatMain)
                )
            )
            .Act.Pass() // pass priority around
            // * beginning of combat phase
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.BeginningOfCombat)
                )
            )
            .Act.Pass() // pass priority around
            // * declare attackers
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.DeclareAttackers)
                )
            )
            .Act.Pass() // pass priority around
            // * end of combat
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Combat)
                    .CurrentStep(StepType.EndOfCombat)
                )
            )
            .Act.Pass() // pass priority around
            // * postcombat main
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.PostcombatMain)
                )
            )
            .Act.Pass() // pass priority around
            // * end
            .Act.Assert(a => a
                .AssertMatch(ma => ma
                    .CurrentPhase(PhaseType.Ending)
                    .CurrentStep(StepType.End)
                )
            )
            .Act.Pass() // pass priority around
            .SetDeck(deck);

        // Act
        var match = new TestMatchWrapper(
            new MatchConfigBuilder()
                .FirstPlayerIdx(0)
                .GameLossIfRequiredToDrawFromEmptyLibrary(false)
                .Build(),
            [ 
                p1, 
                notP1("p2", 1)
                    .Act.Crash(), 
                notP1("p3", 2), 
                notP1("p4", 3) 
            ]
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
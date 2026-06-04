using Mattock.Core.Matches.StateBasedActions;

namespace Mattock.Core.Tests.Rules.StateBasedActions;

public class CheckerSBA : IStateBasedAction
{
    public int Called { get; private set; } = 0;

    public bool Apply(Match match)
    {
        ++Called;
        return false;
    }
}

public class StateBasedActionTests
{
    [Trait("Rules", "704.3.")]
    [Theory]
    [InlineData(StepType.Upkeep, 1 * 2)]
    [InlineData(StepType.Draw, 3 * 2)]
    [InlineData(StepType.BeginningOfCombat, 7 * 2)] // +2 for main phase
    [InlineData(StepType.DeclareAttackers, 9 * 2)]
    [InlineData(StepType.EndOfCombat, 11 * 2)] // +2 for main phase
    [InlineData(StepType.End, 15 * 2)]
    public async Task CheckCalledAmount(StepType skipToStep, int expectedCalled)
    {
        // Arrange
        var deck = new DeckTemplate()
        {
            MainDeck = []
        };

        var p1 = new TestPlayerControllerBuilder("p1")
            .ChoosePlayer.WithIdx(0)
            .SetDeck(deck)
            .Act.AutoPassToStep(skipToStep)            
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
        var checker = new CheckerSBA();
        match.PreLaunchActions.Add(
            m => m.StateBasedActions.StateBasedActions.Add(checker)
        );
        match.RemoveMulligans();

        await match.Run();

        // Assert
        match.Assert(a => a
            .CrashedIntentially()
            .NoChoicesLeft()
        );

        checker.Called.ShouldBe(expectedCalled);
    }

    // TODO add test for checking if state-based actions were checked during cleanup step
}
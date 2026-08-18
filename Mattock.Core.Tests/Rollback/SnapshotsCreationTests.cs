using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Rollback;

public class SnapshotsCreationTests
{
    private static void SnapshotCount(int v, CommandChoicesBuilder.Asserts a)
    {
        a.AssertMatch(am => am
            .AssertSnapshots(asn => asn
                .HasCount(v)
            )
        );
    }

    [Fact]
    public async Task TurnStartSnapshotCreationChecks()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = []
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToStep(StepType.Upkeep)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToStep(StepType.End)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToTurn(2)
            .Act.Assert(a => SnapshotCount(2, a))
            .Act.AutoPassToStep(StepType.End)
            .Act.Assert(a => SnapshotCount(2, a))
            .Act.AutoPassToTurn(3)
            .Act.Assert(a => SnapshotCount(3, a))
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
            // TODO
        );
    }

    [Fact]
    public async Task SnapshotMemoryRestrictionCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .FirstPlayerIdx(0)
            .NoManaPoolEmptying()
            .GameLossIfRequiredToDrawFromEmptyLibrary(false)
            .SnapshotMemory(1)
            .Build();
        
        var deck = new DeckTemplate()
        {
            MainDeck = []
        };

        var p1 = new TestPlayerControllerBuilder("p1", 0)
            .SetDeck(deck)
            .ChoosePlayers.Me()
            .Act.AutoPassToStep(StepType.Upkeep)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToStep(StepType.End)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToTurn(2)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToStep(StepType.End)
            .Act.Assert(a => SnapshotCount(1, a))
            .Act.AutoPassToTurn(3)
            .Act.Assert(a => SnapshotCount(1, a))
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
            // TODO
        );
    }

    // TODO check that a snapshot is created before activating a ability with a cost
    // TODO check that SnapshotMemory works
}
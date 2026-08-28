using Mattock.Core.Matches.Snapshots;

namespace Mattock.Core.Tests.Setup.Asserts;

public class SnapshotAsserts(
    Snapshot snapshot
)
{
    public SnapshotAsserts TurnNumber(int expected)
    {
        snapshot.Meta.TurnCounter.ShouldBe(expected);
        return this;
    }
}
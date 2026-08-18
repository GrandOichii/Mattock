using Mattock.Core.Matches.Snapshots;

namespace Mattock.Core.Tests.Setup.Asserts;

public class SnapshotAsserts(
    Snapshot snapshot
)
{
    public SnapshotAsserts HasId(string expected)
    {
        snapshot.Id.ShouldBe(expected);
        return this;
    }
}
using Mattock.Core.Matches.Snapshots;

namespace Mattock.Core.Tests.Setup.Asserts;

public class SnapshotsAsserts(
    SnapshotsManager manager
)
{
    public SnapshotsAsserts HasCount(int v)
    {
        manager.Snapshots.Count.ShouldBe(v);
        return this;
    }
}
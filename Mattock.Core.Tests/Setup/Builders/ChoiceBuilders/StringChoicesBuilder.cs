namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class StringChoicesBuilder 
    : ChoicesBuilder<TestPlayerController.StringChoice>
{
    public RollbackChoicesBuilder Rollback { get; }

    public StringChoicesBuilder(TestPlayerControllerBuilder builder) : base(builder)
    {
        Rollback = new(this);
    }

    public TestPlayerControllerBuilder Yes()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond("Yes"),
                true
            );
        });
    }

    public TestPlayerControllerBuilder No()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond("No"),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Choose(string choice)
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond(choice),
                true
            );
        });
    }

    
    // TODO repeated code
    public class RollbackChoicesBuilder(StringChoicesBuilder mcb)
    {
        public TestPlayerControllerBuilder ToLast()
        {
            return mcb.Enqueue(
                async (player, options, hint, allowNone) =>
                {
                    var id = player.Match.Session.Snapshots.Snapshots.Last().Id;
                    return (
                        (null!, new() { RequestedSnapshotId = id} ),
                        true
                    );
                }
            );
        }
    }
}


namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class PlayersChoicesBuilder
    : ChoicesBuilder<TestPlayerController.PlayersChoice>
{
    public RollbackChoicesBuilder Rollback { get; }

    public PlayersChoicesBuilder(TestPlayerControllerBuilder builder) : base(builder)
    {
        Rollback = new(this);
    }

    public TestPlayerControllerBuilder WithIdx(int idx)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Player[]>([options.Single(p => p.Idx == idx)]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Me()
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Player[]>([player]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            action(new(player, options, min, max, hint));
            return (([], null), false);
        });
    }
    
    public class Asserts(
        Player player,
        Player[] options,
        int min,
        int max,
        string hint
    )
    {
        public Asserts NonNull()
        {
            player.ShouldNotBeNull();
            options.ShouldNotBeNull();
            hint.ShouldNotBeNull();
            return this;
        }

        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }

    // TODO repeated code
    public class RollbackChoicesBuilder(PlayersChoicesBuilder mcb)
    {
        public TestPlayerControllerBuilder ToLast()
        {
            return mcb.Enqueue(
                async (player, options, min, max, hint) =>
                {
                    var id = player.Match.Session.Snapshots.Snapshots.Last().Id;
                    return (
                        ([], new() { RequestedSnapshotId = id} ),
                        true
                    );
                }
            );
        }
    }
    
}

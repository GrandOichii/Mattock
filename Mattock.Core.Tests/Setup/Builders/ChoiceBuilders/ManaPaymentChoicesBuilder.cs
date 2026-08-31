using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class ManaPaymentChoicesBuilder
    : ChoicesBuilder<TestPlayerController.ManaPaymentChoice>
{
    public RollbackChoicesBuilder Rollback { get; }

    public ManaPaymentChoicesBuilder(TestPlayerControllerBuilder builder) : base(builder)
    {
        Rollback = new(this);
    }

    public TestPlayerControllerBuilder NTimes(int n, Action<ManaPaymentChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(this);
        return _builder;
    }

    public TestPlayerControllerBuilder First()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (
                Respond(options.First()),
                true
            );
        });
    }

    public TestPlayerControllerBuilder FirstOfType(ManaType type)
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (
                Respond(options.First(o => o is StoredManaPaymentChoice m && m.Mana.Type == type)),
                true
            );
        });
    }

    public TestPlayerControllerBuilder ActivateFirst()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (
                Respond(options.First(o => o is ManaAbilityManaPaymentChoice)),
                true
            );
        });
    }

    public TestPlayerControllerBuilder FirstStored()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (
                Respond(options.First(o => o is StoredManaPaymentChoice)),
                true
            );
        });
    }


    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint) =>
        {
            action(new(player, options, hint));
            return ((null!, null), false);
        });
    }
    
    public class Asserts(Player player, IManaPaymentChoice[] options, string hint)
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
    public class RollbackChoicesBuilder(ManaPaymentChoicesBuilder mcb)
    {
        public TestPlayerControllerBuilder ToLast()
        {
            return mcb.Enqueue(
                async (player, options, hint) =>
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


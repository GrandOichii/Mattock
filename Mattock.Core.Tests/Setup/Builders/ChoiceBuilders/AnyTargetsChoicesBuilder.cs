namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

// AnyTargetsChoicesBuilder

public class AnyTargetsChoicesBuilder
    : ChoicesBuilder<TestPlayerController.AnyTargetsChoice>
{
    // public RollbackChoicesBuilder Rollback { get; }

    public AnyTargetsChoicesBuilder(TestPlayerControllerBuilder builder) : base(builder)
    {
        // Rollback = new(this);
    }

    // public TestPlayerControllerBuilder WithIdx(int idx)
    // {
    //     return Enqueue(async (player, options, min, max, hint) =>
    //     {
    //         return (
    //             Respond<Player[]>([options.Single(p => p.Idx == idx)]),
    //             true
    //         );
    //     });
    // }

    public TestPlayerControllerBuilder Me()
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<IAnyTargetChoice[]>([
                    options.Single(o => o.ToUniqueString() == new PlayerAnyTargetChoice(player).ToUniqueString())
                ]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder PlayerWithIdx(int idx)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<IAnyTargetChoice[]>([
                    options.Single(o => o.ToUniqueString() == new PlayerAnyTargetChoice(player.Match.Players[idx]).ToUniqueString())
                ]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder PermanentWithName(string name)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            var permanent = player.Match.Battlefield.GetPermanents().Single(p => p.HasName(name));
            return (
                Respond<IAnyTargetChoice[]>([
                    options.Single(o => o.ToUniqueString() == new PermanentAnyTargetChoice(permanent).ToUniqueString())
                ]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Crash()
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            throw new IntentionalCrashException();
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
        IAnyTargetChoice[] options,
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
            min.ShouldBePositive();
            max.ShouldBePositive();
            return this;
        }

        public Asserts CanTargetPlayer(int idx)
        {
            var p = player.Match.Players[idx];
            var playerTarget = new PlayerAnyTargetChoice(p).ToUniqueString();
            
            options.ShouldContain(o => o.ToUniqueString() == playerTarget);
            return this;
        }

        public Asserts CanTargetPermanent(string name)
        {
            var p = player.Match.Battlefield.GetPermanents().Single(p => p.HasName(name));
            var permanentTarget = new PermanentAnyTargetChoice(p).ToUniqueString();
            
            options.ShouldContain(o => o.ToUniqueString() == permanentTarget);
            return this;
        }

        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }
}

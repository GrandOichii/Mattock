using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class ManaPaymentChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.ManaPaymentChoice>(builder)
{
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
}


namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class CardChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.CardChoice>(builder)
{
    public TestPlayerControllerBuilder NTimes(int n, Action<int, CardChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(i, this);
        return _builder;
    }

    public TestPlayerControllerBuilder First()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond(options[0]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder FirstWithName(string name)
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond(options.First(c => c.HasName(name))),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            action(new(player, options, hint));
            return ((null, null), false);
        });
    }
    
    public class Asserts(Player player, Card[] options, string hint)
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

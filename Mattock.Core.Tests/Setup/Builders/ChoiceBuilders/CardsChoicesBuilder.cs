namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class CardsChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.CardsChoice>(builder)
{
    public TestPlayerControllerBuilder NTimes(int n, Action<int, CardsChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(i, this);
        return _builder;
    }

    public TestPlayerControllerBuilder First()
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Card[]>([ options[0] ]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder FirstN(int n)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Card[]>([ ..options[0..n] ]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder FirstWithName(string name)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Card[]>([ options.First(c => c.HasName(name)) ]),
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
    
    public class Asserts(Player player, Card[] options, int min, int max, string hint)
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

        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }
}

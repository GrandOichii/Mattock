using Mattock.Core.Matches.Permanents;

namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class PermanentsChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.PermanentsChoice>(builder)
{
    public TestPlayerControllerBuilder WithName(string name)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return (
                Respond<Permanent[]>([options.Single(p => p.HasName(name))]),
                true
            );
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            action(new(player, options, min, max, hint));
            return (
                Respond<Permanent[]>([]),
                false
            );
        });
    }
    
    public class Asserts(Player player, Permanent[] options, int min, int max, string hint)
    {
        public Asserts NonNull()
        {
            player.ShouldNotBeNull();
            options.ShouldNotBeNull();
            hint.ShouldNotBeNull();
            max.ShouldBeGreaterThan(min);
            return this;
        }
        
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }

}

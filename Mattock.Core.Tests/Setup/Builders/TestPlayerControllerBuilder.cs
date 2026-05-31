namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;

    public PlayerChoicesBuilder PlayerChoices { get; }

    public TestPlayerControllerBuilder(string name)
    {
        _name = name;
        PlayerChoices = new(this);
    }

    public PlayerChoicesBuilder ChoosePlayer => PlayerChoices;

    public TestPlayerController Build()
    {
        return new(
            _name,
            PlayerChoices.Queue
        );
    }
}

public abstract class ChoicesBuilder<TDelegate>(TestPlayerControllerBuilder builder)
{
    protected readonly TestPlayerControllerBuilder _builder = builder;
    public Queue<TDelegate> Queue { get; } = new();

    public TestPlayerControllerBuilder Enqueue(TDelegate choice)
    {
        Queue.Enqueue(choice);
        return _builder;
    }
}

public class PlayerChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.PlayerChoice>(builder)
{
    public TestPlayerControllerBuilder WithIdx(int idx)
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.Single(p => p.Idx == idx), true);
        });
    }
}
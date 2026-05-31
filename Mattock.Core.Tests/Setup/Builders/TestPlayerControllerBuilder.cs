using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;
    private DeckTemplate _deck;

    public PlayerChoicesBuilder PlayerChoices { get; }

    public TestPlayerControllerBuilder(string name)
    {
        _name = name;
        _deck = new()
        {
            MainDeck = []
        };
        PlayerChoices = new(this);
    }

    public PlayerChoicesBuilder ChoosePlayer => PlayerChoices;

    public TestPlayerControllerBuilder SetDeck(DeckTemplate deck)
    {
        _deck = deck;
        return this;
    }

    public TestPlayerController Build()
    {
        return new(
            _name,
            _deck,
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
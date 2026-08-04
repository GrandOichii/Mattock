using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public abstract class ChoicesBuilder<TDelegate>(TestPlayerControllerBuilder builder)
{
    protected readonly TestPlayerControllerBuilder _builder = builder;
    public Queue<TDelegate> Queue { get; } = new();

    public TestPlayerControllerBuilder Enqueue(TDelegate choice)
    {
        Queue.Enqueue(choice);
        return _builder;
    }

    protected (T, RollbackRequest?) Respond<T>(T o) => (o, null);
    protected (T?, RollbackRequest?) RespondNull<T>() where T : class => (null, null);
}

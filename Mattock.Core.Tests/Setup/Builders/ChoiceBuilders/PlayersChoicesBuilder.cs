namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class PlayersChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.PlayersChoice>(builder)
{
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
}

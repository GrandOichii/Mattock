namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;


public class StringChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.StringChoice>(builder)
{
    public TestPlayerControllerBuilder Yes()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond("Yes"),
                true
            );
        });
    }

    public TestPlayerControllerBuilder No()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (
                Respond("No"),
                true
            );
        });
    }
}


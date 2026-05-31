using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Tests.Setup;

public class TestPlayerController(
    string name,
    DeckTemplate deck,
    Queue<TestPlayerController.PlayerChoice> playerChoices
    ) : IPlayerController
{
    // public delegate Task<(string?, bool)> PlayerAction(TestMatch match, Player player, List<string> options);
    public delegate Task<(Player?, bool)> PlayerChoice(Player player, Player[] options, string hint);

    public void AssertNoChoicesLeft()
    {
        playerChoices.Count.ShouldBe(0, $"{nameof(PlayerChoice)} queue of player {name} is not empty");
    }

    public PlayerSetup GetPlayerSetup()
    {
        return new()
        {
            Name = name,
            Controller = this,
            Deck = deck,
        };
    }

    public static async Task<TResult> Dequeue<TResult, TDelegate>(
        Player player,
        TResult[] options,
        string hint,
        Func<TDelegate, Player, TResult[], string, Task<(TResult?, bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options, hint);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null gig choice for {methodName}");
            return result;
        }
        
        throw new Exception($"No gig choices left in queue for player {player.GetDisplayName} (hint: {hint})");
    }

    public async Task<Player> ChoosePlayer(Player player, Player[] options, string hint)
    {
        return await Dequeue(
            player,
            options,
            hint,
            (d, p, o, h) => d(p, o, h),
            playerChoices,
            nameof(ChoosePlayer)
        );
    }
}

public class IntentionalCrashException : Exception
{
    public IntentionalCrashException() { }
    public IntentionalCrashException(string message) : base(message) { }
    public IntentionalCrashException(string message, Exception inner) : base(message, inner) { }
}
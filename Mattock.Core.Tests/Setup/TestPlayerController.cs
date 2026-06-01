using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Tests.Setup;

public class TestPlayerController(
    TestMatchWrapper match,
    string name,
    DeckTemplate deck,
    Queue<(TestPlayerController.CommandChoice, bool)> commandChoices,
    Queue<TestPlayerController.PlayerChoice> playerChoices,
    Queue<TestPlayerController.StringChoice> stringChoices,
    Queue<TestPlayerController.CardChoice> cardChoices
    ) : IPlayerController
{
    public delegate Task<(ICommand?, bool, bool)> CommandChoice(TestMatchWrapper match, Player player, ICommand[] options);
    public delegate Task<(Player?, bool)> PlayerChoice(Player player, Player[] options, string hint);
    public delegate Task<(string?, bool)> StringChoice(Player player, string[] options, string hint);
    public delegate Task<(Card?, bool)> CardChoice(Player player, Card[] options, string hint);

    public void AssertNoChoicesLeft(
        bool checkCommandChoices,
        bool checkPlayerChoices,
        bool checkStringChoices,
        bool checkCardChoices
    )
    {
        if (checkPlayerChoices)
            playerChoices.Count.ShouldBe(0, $"{nameof(PlayerChoice)} queue of player {name} is not empty (size: {playerChoices.Count})");

        if (checkStringChoices)
            stringChoices.Count.ShouldBe(0, $"{nameof(StringChoice)} queue of player {name} is not empty (size: {stringChoices.Count})");

        if (checkCardChoices)
            cardChoices.Count.ShouldBe(0, $"{nameof(CardChoice)} queue of player {name} is not empty (size: {cardChoices.Count})");

        if (checkCommandChoices)
        {
            var c = commandChoices.Count(c => c.Item2);
            c.ShouldBe(0, $"{nameof(CommandChoice)} queue of player {name} contains essential commands (amount: {c})");
        }
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

    public async Task<ICommand> ChooseCommand(Player player, ICommand[] options)
    {
        while (commandChoices.Count > 0)
        {
            var choice = commandChoices.Peek().Item1;
            var (result, isResult, removeFromQueue) = await choice(match, player, options);
            if (removeFromQueue)
                commandChoices.Dequeue();
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null choice for {nameof(ChooseCommand)} of player {player.GetDisplayName()}");
            return result;
        }
        
        throw new Exception($"No choices left in queue for {nameof(ChooseCommand)} of player {player.GetDisplayName()}");
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
            if (result is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result;
        }
        
        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()} (hint: {hint})");
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
    
    public async Task<string> ChooseString(Player player, string[] options, string hint)
    {
        return await Dequeue(
            player,
            options,
            hint,
            (d, p, o, h) => d(p, o, h),
            stringChoices,
            nameof(ChooseString)
        );
    }

    public async Task<Card> ChooseCard(Player player, Card[] options, string hint)
    {
        return await Dequeue(
            player,
            options,
            hint,
            (d, p, o, h) => d(p, o, h),
            cardChoices,
            nameof(ChooseString)
        );
    }

    public Task Update(Player player, string? msg) => Task.CompletedTask;
}

public class IntentionalCrashException : Exception
{
    public IntentionalCrashException() { }
    public IntentionalCrashException(string message) : base(message) { }
    public IntentionalCrashException(string message, Exception inner) : base(message, inner) { }
}
using Mattock.Core.Matches.Players.Costs;

namespace Mattock.Core.Tests.Setup;

public class TestPlayerController(
    TestMatchWrapper match,
    string name,
    DeckTemplate deck,
    int teamIdx,
    Queue<(TestPlayerController.CommandChoice, bool)> commandChoices,
    Queue<TestPlayerController.PlayerChoice> playerChoices,
    Queue<TestPlayerController.StringChoice> stringChoices,
    Queue<TestPlayerController.CardChoice> cardChoices,
    Queue<TestPlayerController.CostCollectionChoice> costCollectionChoices,
    Queue<TestPlayerController.StoredManaChoice> storedManaChoices,
    Queue<TestPlayerController.AttackDeclarationsChoice> attackDeclarationsChoices,
    Queue<TestPlayerController.BlockDeclarationsChoice> blockDeclarationsChoices
) : IPlayerController
{
    public delegate Task<(ICommand?, bool, bool)> CommandChoice(TestMatchWrapper match, Player player, ICommand[] options);
    public delegate Task<(Player?, bool)> PlayerChoice(Player player, Player[] options, string hint, bool allowNone);
    public delegate Task<(string?, bool)> StringChoice(Player player, string[] options, string hint, bool allowNone);
    public delegate Task<(Card?, bool)> CardChoice(Player player, Card[] options, string hint, bool allowNone);
    public delegate Task<(CostCollection?, bool)> CostCollectionChoice(Player player, CostCollection[] options, string hint, bool allowNone);
    public delegate Task<(StoredMana?, bool)> StoredManaChoice(Player player, StoredMana[] options, string hint, bool allowNone);
    public delegate Task<(AttackDeclaration[]?, bool)> AttackDeclarationsChoice(Player player, AttackDeclaration[] options);
    public delegate Task<(BlockDeclaration[]?, bool)> BlockDeclarationsChoice(Player player, BlockDeclaration[] options);

    public void AssertNoChoicesLeft(
        bool checkCommandChoices,
        bool checkPlayerChoices,
        bool checkStringChoices,
        bool checkCardChoices,
        bool checkCostCollectionChoices,
        bool checkStoredManaChoices,
        bool checkAttackDeclarationChoices,
        bool checkBlockDeclarationChoices
    )
    {
        if (checkPlayerChoices)
            playerChoices.Count.ShouldBe(0, $"{nameof(PlayerChoice)} queue of player {name} is not empty (size: {playerChoices.Count})");

        if (checkStringChoices)
            stringChoices.Count.ShouldBe(0, $"{nameof(StringChoice)} queue of player {name} is not empty (size: {stringChoices.Count})");

        if (checkCardChoices)
            cardChoices.Count.ShouldBe(0, $"{nameof(CardChoice)} queue of player {name} is not empty (size: {cardChoices.Count})");

        if (checkCostCollectionChoices)
            costCollectionChoices.Count.ShouldBe(0, $"{nameof(CostCollectionChoice)} queue of player {name} is not empty (size: {costCollectionChoices.Count})");

        if (checkStoredManaChoices)
            storedManaChoices.Count.ShouldBe(0, $"{nameof(StoredManaChoice)} queue of player {name} is not empty (size: {storedManaChoices.Count})");

        if (checkAttackDeclarationChoices)
            attackDeclarationsChoices.Count.ShouldBe(0, $"{nameof(AttackDeclarationsChoice)} queue of player {name} is not empty (size: {attackDeclarationsChoices.Count})");

        if (checkBlockDeclarationChoices)
            blockDeclarationsChoices.Count.ShouldBe(0, $"{nameof(BlockDeclarationsChoice)} queue of player {name} is not empty (size: {attackDeclarationsChoices.Count})");

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
            TeamIdx = teamIdx,
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
        bool allowNone,
        Func<TDelegate, Player, TResult[], string, bool, Task<(TResult?, bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options, hint, allowNone);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result;
        }

        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()} (hint: {hint})");
    }

    public async Task<Player?> ChoosePlayer(Player player, Player[] options, string hint, bool allowNone)
    {
        return await Dequeue(
            player,
            options,
            hint,
            allowNone,
            (d, p, o, h, a) => d(p, o, h, a),
            playerChoices,
            nameof(ChoosePlayer)
        );
    }

    public async Task<string?> ChooseString(Player player, string[] options, string hint, bool allowNone)
    {
        return await Dequeue(
            player,
            options,
            hint,
            allowNone,
            (d, p, o, h, a) => d(p, o, h, a),
            stringChoices,
            nameof(ChooseString)
        );
    }

    public async Task<Card?> ChooseCard(Player player, Card[] options, string hint, bool allowNone)
    {
        return await Dequeue(
            player,
            options,
            hint,
            allowNone,
            (d, p, o, h, a) => d(p, o, h, a),
            cardChoices,
            nameof(ChooseCard)
        );
    }

    public async Task<CostCollection?> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
    {
        return await Dequeue(
            player,
            options,
            hint,
            allowNone,
            (d, p, o, h, a) => d(p, o, h, a),
            costCollectionChoices,
            nameof(ChooseCostCollection)
        );
    }

    public async Task<StoredMana?> ChooseStoredMana(Player player, StoredMana[] options, string hint, bool allowNone)
    {
        return await Dequeue(
            player,
            options,
            hint,
            allowNone,
            (d, p, o, h, a) => d(p, o, h, a),
            storedManaChoices,
            nameof(ChooseStoredMana)
        );
    }

    public Task Update(Player player, string? msg) => Task.CompletedTask;

    public async Task<AttackDeclaration[]> ChooseAttackDeclarations(Player player, AttackDeclaration[] options)
    {
        while (attackDeclarationsChoices.Count > 0)
        {
            var choice = attackDeclarationsChoices.Dequeue();
            var (result, isResult) = await choice(player, options);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null choice for {nameof(ChooseAttackDeclarations)} of player {player.GetDisplayName()}");
            return result;
        }

        throw new Exception($"No choices left in queue for {nameof(ChooseAttackDeclarations)} of player {player.GetDisplayName()}");
    }

    public async Task<BlockDeclaration[]> ChooseBlockDeclarations(Player player, BlockDeclaration[] options)
    {
        while (blockDeclarationsChoices.Count > 0)
        {
            var choice = blockDeclarationsChoices.Dequeue();
            var (result, isResult) = await choice(player, options);
            if (!isResult) continue;
            if (result is null) throw new Exception($"Provided null choice for {nameof(ChooseBlockDeclarations)} of player {player.GetDisplayName()}");
            return result;
        }

        throw new Exception($"No choices left in queue for {nameof(ChooseBlockDeclarations)} of player {player.GetDisplayName()}");
    }
}

public class IntentionalCrashException : Exception
{
    public IntentionalCrashException() { }
    public IntentionalCrashException(string message) : base(message) { }
    public IntentionalCrashException(string message, Exception inner) : base(message, inner) { }
}
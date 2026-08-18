using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Tests.Setup;

public class TestPlayerController(
    TestMatchWrapper match,
    string name,
    DeckTemplate deck,
    int teamIdx,
    Queue<(TestPlayerController.CommandChoice, bool)> commandChoices,
    Queue<TestPlayerController.PlayersChoice> playersChoices,
    Queue<TestPlayerController.PermanentsChoice> permanentsChoices,
    Queue<TestPlayerController.StringChoice> stringChoices,
    Queue<TestPlayerController.CardChoice> cardChoices,
    Queue<TestPlayerController.CostCollectionChoice> costCollectionChoices,
    Queue<TestPlayerController.ManaPaymentChoice> manaPaymentChoices,
    Queue<TestPlayerController.AttackDeclarationsChoice> attackDeclarationsChoices,
    Queue<TestPlayerController.BlockDeclarationsChoice> blockDeclarationsChoices
) : IPlayerController
{
    public delegate Task<((ICommand?, RollbackRequest?), bool, bool)> CommandChoice(TestMatchWrapper match, Player player, ICommand[] options);
    public delegate Task<((Player[], RollbackRequest?), bool)> PlayersChoice(Player player, Player[] options, int min, int max, string hint);
    public delegate Task<((Permanent[], RollbackRequest?), bool)> PermanentsChoice(Player player, Permanent[] options, int min, int max, string hint);
    public delegate Task<((string?, RollbackRequest?), bool)> StringChoice(Player player, string[] options, string hint, bool allowNone);
    public delegate Task<((Card?, RollbackRequest?), bool)> CardChoice(Player player, Card[] options, string hint, bool allowNone);
    public delegate Task<((CostCollection?, RollbackRequest?), bool)> CostCollectionChoice(Player player, CostCollection[] options, string hint, bool allowNone);
    public delegate Task<((IManaPaymentChoice, RollbackRequest?), bool)> ManaPaymentChoice(Player player, IManaPaymentChoice[] options, string hint);
    public delegate Task<((AttackDeclaration[]?, RollbackRequest?), bool)> AttackDeclarationsChoice(Player player, AttackDeclaration[] options);
    public delegate Task<((BlockDeclaration[]?, RollbackRequest?), bool)> BlockDeclarationsChoice(Player player, BlockDeclaration[] options);

    public void AssertNoChoicesLeft(
        bool checkCommandChoices,
        bool checkPlayersChoices,
        bool checkPermanentsChoices,
        bool checkStringChoices,
        bool checkCardChoices,
        bool checkCostCollectionChoices,
        bool checkManaPaymentChoices,
        bool checkAttackDeclarationChoices,
        bool checkBlockDeclarationChoices
    )
    {
        if (checkPlayersChoices)
            playersChoices.Count.ShouldBe(0, $"{nameof(PlayersChoice)} queue of player {name} is not empty (size: {playersChoices.Count})");

        if (checkPermanentsChoices)
            permanentsChoices.Count.ShouldBe(0, $"{nameof(PermanentsChoice)} queue of player {name} is not empty (size: {permanentsChoices.Count})");

        if (checkStringChoices)
            stringChoices.Count.ShouldBe(0, $"{nameof(StringChoice)} queue of player {name} is not empty (size: {stringChoices.Count})");

        if (checkCardChoices)
            cardChoices.Count.ShouldBe(0, $"{nameof(CardChoice)} queue of player {name} is not empty (size: {cardChoices.Count})");

        if (checkCostCollectionChoices)
            costCollectionChoices.Count.ShouldBe(0, $"{nameof(CostCollectionChoice)} queue of player {name} is not empty (size: {costCollectionChoices.Count})");

        if (checkManaPaymentChoices)
            manaPaymentChoices.Count.ShouldBe(0, $"{nameof(ManaPaymentChoice)} queue of player {name} is not empty (size: {manaPaymentChoices.Count})");

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

    public async Task<(ICommand, RollbackRequest?)> ChooseCommand(Player player, ICommand[] options)
    {
        while (commandChoices.Count > 0)
        {
            var choice = commandChoices.Peek().Item1;
            var (result, isResult, removeFromQueue) = await choice(match, player, options);
            if (removeFromQueue)
                commandChoices.Dequeue();
            if (!isResult) continue;
            // if (result.Item1 is null) throw new Exception($"Provided null choice for {nameof(ChooseCommand)} of player {player.GetDisplayName()}");
            return result!; // TODO sus
        }

        throw new Exception($"No choices left in queue for {nameof(ChooseCommand)} of player {player.GetDisplayName()}");
    }

    public static async Task<(TResult, RollbackRequest?)> Dequeue<TResult, TDelegate>(
        Player player,
        TResult[] options,
        string hint,
        bool allowNone,
        Func<TDelegate, Player, TResult[], string, bool, Task<((TResult?, RollbackRequest?), bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options, hint, allowNone);
            if (!isResult) continue;
            // if (result.Item1 is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result!;
        }

        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()} (hint: {hint})");
    }

    public static async Task<(TResult[], RollbackRequest?)> Dequeue<TResult, TDelegate>(
        Player player,
        TResult[] options,
        Func<TDelegate, Player, TResult[], Task<((TResult[]?, RollbackRequest?), bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options);
            if (!isResult) continue;
            // if (result.Item1 is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result!;
        }

        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()}");
    }

    public static async Task<(TResult, RollbackRequest?)> Dequeue<TResult, TDelegate>(
        Player player,
        TResult[] options,
        string hint,
        Func<TDelegate, Player, TResult[], string, Task<((TResult, RollbackRequest?), bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options, hint);
            if (!isResult) continue;
            // if (result.Item1 is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result;
        }

        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()} (hint: {hint})");
    }

    public static async Task<(TResult[], RollbackRequest?)> Dequeue<TResult, TDelegate>(
        Player player,
        TResult[] options,
        int min,
        int max,
        string hint,
        Func<TDelegate, Player, TResult[], int, int, string, Task<((TResult[], RollbackRequest?), bool)>> getter,
        Queue<TDelegate> queue,
        string methodName
    )
    {
        while (queue.Count > 0)
        {
            var choice = queue.Dequeue();
            var (result, isResult) = await getter(choice, player, options, min, max, hint);
            if (!isResult) continue;
            // if (result.Item1 is null) throw new Exception($"Provided null choice for {methodName} of player {player.GetDisplayName()}");
            return result;
        }

        throw new Exception($"No choices left in queue for {methodName} of player {player.GetDisplayName()} (hint: {hint})");
    }

    public async Task<(Player[], RollbackRequest?)> ChoosePlayers(Player player, Player[] options, int min, int max, string hint)
    {
        return await Dequeue(
            player,
            options,
            min,
            max,
            hint,
            (d, p, o, mmin, mmax, h) => d(p, o, mmin, mmax, h),
            playersChoices,
            nameof(ChoosePlayers)
        );
    }

    public async Task<(Permanent[], RollbackRequest?)> ChoosePermanents(Player player, Permanent[] options, int min, int max, string hint)
    {
        return await Dequeue(
            player,
            options,
            min,
            max,
            hint,
            (d, p, o, mmin, mmax, h) => d(p, o, mmin, mmax, h),
            permanentsChoices,
            nameof(ChoosePermanents)
        );
    }

    public async Task<(string?, RollbackRequest?)> ChooseString(Player player, string[] options, string hint, bool allowNone)
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

    public async Task<(Card?, RollbackRequest?)> ChooseCard(Player player, Card[] options, string hint, bool allowNone)
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

    public async Task<(CostCollection?, RollbackRequest?)> ChooseCostCollection(Player player, CostCollection[] options, string hint, bool allowNone)
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

    public async Task<(IManaPaymentChoice, RollbackRequest?)> ChooseManaPayment(Player player, IManaPaymentChoice[] options, string hint)
    {
        return await Dequeue(
            player,
            options,
            hint,
            (d, p, o, h) => d(p, o, h),
            manaPaymentChoices,
            nameof(ChooseManaPayment)
        );
    }

    public Task Update(Player player, string? msg) => Task.CompletedTask;

    public async Task<(AttackDeclaration[], RollbackRequest?)> ChooseAttackDeclarations(Player player, AttackDeclaration[] options)
    {
        return await Dequeue(
            player,
            options,
            (d, p, o) => d(p, o),
            attackDeclarationsChoices,
            nameof(ChooseAttackDeclarations)
        );
    }

    public async Task<(BlockDeclaration[], RollbackRequest?)> ChooseBlockDeclarations(Player player, BlockDeclaration[] options)
    {
        return await Dequeue(
            player,
            options,
            (d, p, o) => d(p, o),
            blockDeclarationsChoices,
            nameof(ChooseBlockDeclarations)
        );
    }

    public Task<bool> ApproveRollback(Player player, string hint)
    {
        // TODO
        return Task.FromResult(true);
    }
}

public class IntentionalCrashException : Exception
{
    public IntentionalCrashException() { }
    public IntentionalCrashException(string message) : base(message) { }
    public IntentionalCrashException(string message, Exception inner) : base(message, inner) { }
}
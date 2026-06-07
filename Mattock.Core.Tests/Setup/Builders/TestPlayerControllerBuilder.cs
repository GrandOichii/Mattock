using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;
    private DeckTemplate _deck;
    private int _teamIdx;

    public CommandChoicesBuilder CommandChoices { get; }
    public PlayerChoicesBuilder PlayerChoices { get; }
    public StringChoicesBuilder StringChoices { get; }
    public CardChoicesBuilder CardChoices { get; }
    public CostCollectionChoicesBuilder CostCollectionChoices { get; }
    public StoredManaChoicesBuilder StoredManaChoices { get; }

    public TestPlayerControllerBuilder(string name, int teamIdx)
    {
        _name = name;
        _teamIdx = teamIdx;
        _deck = new()
        {
            MainDeck = []
        };

        CommandChoices = new(this);
        PlayerChoices = new(this);
        StringChoices = new(this);
        CardChoices = new(this);
        CostCollectionChoices = new(this);
        StoredManaChoices = new(this);
    }

    public PlayerChoicesBuilder ChoosePlayer => PlayerChoices;
    public StringChoicesBuilder ChooseString => StringChoices;
    public CardChoicesBuilder ChooseCard => CardChoices;
    public CostCollectionChoicesBuilder ChooseCostCollection => CostCollectionChoices;
    public StoredManaChoicesBuilder ChooseMana => StoredManaChoices;
    public CommandChoicesBuilder Act => CommandChoices;

    public TestPlayerControllerBuilder SetDeck(DeckTemplate deck)
    {
        _deck = deck;
        return this;
    }

    public TestPlayerController Build(TestMatchWrapper match)
    {
        return new(
            match,
            _name,
            _deck,
            _teamIdx,
            CommandChoices.Queue,
            PlayerChoices.Queue,
            StringChoices.Queue,
            CardChoices.Queue,
            CostCollectionChoices.Queue,
            StoredManaChoices.Queue
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

public class CommandChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<(TestPlayerController.CommandChoice, bool)>(builder)
{
    
    public TestPlayerControllerBuilder NTimes(int n, Action<CommandChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(this);
        return _builder;
    }

    public TestPlayerControllerBuilder ForEach<T>(IEnumerable<T> list, Action<T, CommandChoicesBuilder> action)
    {
        foreach (var item in list)
            action(item, this);
        return _builder;
    }

    public TestPlayerControllerBuilder Crash()
    {
        return Enqueue((
            async (match, player, options) => throw new IntentionalCrashException(),
            false
        ));
    }

    private static ICommand PassChoice(ICommand[] options) =>
        options.Single(o => o.ToCommandString() == PassAction.ActionWord);

    public TestPlayerControllerBuilder Pass()
    {
        return Enqueue((
            async (match, player, options) =>
            {
                return (PassChoice(options), true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder PlayLandWithName(string name)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                var land = player.GetPlayableLands().First(c => c.HasName(name));
                var command = new PlayLandCommand(land);
                return (command, true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder CastSpellWithName(string name)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                var card = player.GetCastableCards().First(c => c.HasName(name));
                var command = new CastSpellCommand(player, card);
                return (command, true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder AddMana(ManaType type, int amount)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                player.ManaPool.AddGenericMana(type, amount);
                return (null, false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder SetPlayerStatus(int playerIdx, PlayerStatus status, bool silent = false)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                match.Match!.Players[playerIdx].SetStatus(status, silent);
                return (null, false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder CheckForWinners()
    {
        return Enqueue((
            async (match, player, options) =>
            {
                match.Match!.CheckForWinners();
                return (null, false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder AutoPass()
    {
        return Enqueue((
            async (match, player, options) =>
            {
                return (PassChoice(options), true, false);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToStep(StepType step)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                var currentStep = match.Match!.TurnManager.GetCurrentPhase().GetCurrentStep();
                if (currentStep is null || currentStep.Type != step)
                    return (PassChoice(options), true, false);
                return (null, false, true);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToPhase(PhaseType phase)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                var currentPhase = match.Match!.TurnManager.GetCurrentPhase();
                if (currentPhase is null || currentPhase.Type != phase)
                    return (PassChoice(options), true, false);
                return (null, false, true);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToTurn(int turn)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                if (match.Match!.TurnCounter == turn)
                    return (null, false, true);
                return (PassChoice(options), true, false);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder SetLife(int playerIdx, int life)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                match.Match!.Players[playerIdx].Life.Set(life);
                return (null, false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                action(new(match, player, options));
                return (null, false, true);
            },
            true
        ));
    }

    public class Asserts(TestMatchWrapper match, Player player, ICommand[] options)
    {
        public Asserts AssertMatch(Action<MatchAsserts> action)
        {
            action(new(match));
            return this;
        }

        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }

        public Asserts CanPass()
        {
            options.Any(a => a.ToCommandString() == PassAction.ActionWord).ShouldBeTrue(
                $"Player {player.GetDisplayName()} should be able to pass priority"
            );
            return this;
        }

        public Asserts CanPlayLand()
        {
            options.Any(a => a.ToCommandString().StartsWith(PlayLandSpecialAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} should be able to play lands"
            );
            return this;
        }

        public Asserts CanCastSpell()
        {
            options.Any(a => a.ToCommandString().StartsWith(CastSpellAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} should be able to cast spells"
            );
            return this;
        }

        public Asserts CantPlayLand()
        {
            options.All(a => !a.ToCommandString().StartsWith(PlayLandSpecialAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} shouldn't be able to play lands"
            );
            return this;
        }

        public Asserts CantCastSpell()
        {
            options.All(a => !a.ToCommandString().StartsWith(CastSpellAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} shouldn't be able to cast spells"
            );
            return this;
        }
    }
}

public class StringChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.StringChoice>(builder)
{
    public TestPlayerControllerBuilder Yes()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return ("Yes", true);
        });
    }

    public TestPlayerControllerBuilder No()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return ("No", true);
        });
    }
}

public class CardChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.CardChoice>(builder)
{
    public TestPlayerControllerBuilder NTimes(int n, Action<int, CardChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(i, this);
        return _builder;
    }

    public TestPlayerControllerBuilder First()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options[0], true);
        });
    }

    public TestPlayerControllerBuilder FirstWithName(string name)
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.First(c => c.HasName(name)), true);
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint) =>
        {
            action(new(player, options, hint));
            return (null, false);
        });
    }
    
    public class Asserts(Player player, Card[] options, string hint)
    {
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }
}

public class CostCollectionChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.CostCollectionChoice>(builder)
{
    
}

public class StoredManaChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.StoredManaChoice>(builder)
{
    public TestPlayerControllerBuilder NTimes(int n, Action<StoredManaChoicesBuilder> action)
    {
        for (int i = 0; i < n; ++i)
            action(this);
        return _builder;
    }

    public TestPlayerControllerBuilder First()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.First(), true);
        });
    }

    public TestPlayerControllerBuilder FirstOfType(ManaType type)
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.First(o => o.Type == type), true);
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint) =>
        {
            action(new(player, options, hint));
            return (null, false);
        });
    }
    
    public class Asserts(Player player, StoredMana[] options, string hint)
    {
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }
}


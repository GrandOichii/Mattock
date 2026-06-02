using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Turns.Phases;
using Mattock.Core.Matches.Turns.Steps;
using Mattock.Core.Setup.Templates;
using Mattock.Core.Tests.Setup.Asserts;

namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;
    private DeckTemplate _deck;

    public CommandChoicesBuilder CommandChoices { get; }
    public PlayerChoicesBuilder PlayerChoices { get; }
    public StringChoicesBuilder StringChoices { get; }
    public CardChoicesBuilder CardChoices { get; }

    public TestPlayerControllerBuilder(string name)
    {
        _name = name;
        _deck = new()
        {
            MainDeck = []
        };

        CommandChoices = new(this);
        PlayerChoices = new(this);
        StringChoices = new(this);
        CardChoices = new(this);
    }

    public PlayerChoicesBuilder ChoosePlayer => PlayerChoices;
    public StringChoicesBuilder ChooseString => StringChoices;
    public CardChoicesBuilder ChooseCard => CardChoices;
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
            CommandChoices.Queue,
            PlayerChoices.Queue,
            StringChoices.Queue,
            CardChoices.Queue
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
}

using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Scripting.Activated;

namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;
    private DeckTemplate _deck;
    private int _teamIdx;

    public CommandChoicesBuilder CommandChoices { get; }
    public PlayersChoicesBuilder PlayersChoices { get; }
    public PermanentsChoicesBuilder PermanentsChoices { get; }
    public StringChoicesBuilder StringChoices { get; }
    public CardChoicesBuilder CardChoices { get; }
    public CostCollectionChoicesBuilder CostCollectionChoices { get; }
    public ManaPaymentChoicesBuilder ManaPaymentChoices { get; }
    public AttackDeclarationsChoicesBuilder AttackDeclarationsChoices { get; }
    public BlockDeclarationsChoicesBuilder BlockDeclarationsChoices { get; }

    public TestPlayerControllerBuilder(string name, int teamIdx)
    {
        _name = name;
        _teamIdx = teamIdx;
        _deck = new()
        {
            MainDeck = []
        };

        CommandChoices = new(this);
        PlayersChoices = new(this);
        PermanentsChoices = new(this);
        StringChoices = new(this);
        CardChoices = new(this);
        CostCollectionChoices = new(this);
        ManaPaymentChoices = new(this);
        AttackDeclarationsChoices = new(this);
        BlockDeclarationsChoices = new(this);
    }

    public PlayersChoicesBuilder ChoosePlayers => PlayersChoices;
    public PermanentsChoicesBuilder ChoosePermanents => PermanentsChoices;
    public StringChoicesBuilder ChooseString => StringChoices;
    public CardChoicesBuilder ChooseCard => CardChoices;
    public CostCollectionChoicesBuilder ChooseCostCollection => CostCollectionChoices;
    public ManaPaymentChoicesBuilder PayMana => ManaPaymentChoices;
    public CommandChoicesBuilder Act => CommandChoices;
    public AttackDeclarationsChoicesBuilder DeclareAttack => AttackDeclarationsChoices; 
    public BlockDeclarationsChoicesBuilder DeclareBlock => BlockDeclarationsChoices; 

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
            PlayersChoices.Queue,
            PermanentsChoices.Queue,
            StringChoices.Queue,
            CardChoices.Queue,
            CostCollectionChoices.Queue,
            ManaPaymentChoices.Queue,
            AttackDeclarationsChoices.Queue,
            BlockDeclarationsChoices.Queue
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

public class PlayersChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.PlayersChoice>(builder)
{
    public TestPlayerControllerBuilder WithIdx(int idx)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return ([options.Single(p => p.Idx == idx)], true);
        });
    }

    public TestPlayerControllerBuilder Me()
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return ([player], true);
        });
    }
}

public class PermanentsChoicesBuilder(TestPlayerControllerBuilder builder) 
    : ChoicesBuilder<TestPlayerController.PermanentsChoice>(builder)
{
    public TestPlayerControllerBuilder WithName(string name)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            return ([options.Single(p => p.HasName(name))], true);
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, min, max, hint) =>
        {
            action(new(player, options, min, max, hint));
            return ([], false);
        });
    }
    
    public class Asserts(Player player, Permanent[] options, int min, int max, string hint)
    {
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
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

    public TestPlayerControllerBuilder Activate(string permanentName, int abilityIdx = 0)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                ActivatedAbility[] arr = [.. player
                    .GetActivatableAbilities()
                    .Where(a => a.Card.HasName(permanentName))];

                var command = new ActivateAbilityCommand(player, arr[abilityIdx]);
                return (command, true, true);

            },
            true
        ));
    }

    public TestPlayerControllerBuilder ActivateMana(string permanentName, int abilityIdx = 0)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                ActivatedAbility[] arr = [.. player
                    .GetActivatableManaAbilities()
                    .Where(a => a.Card.HasName(permanentName))];

                var command = new ActivateManaAbilityCommand(player, arr[abilityIdx]);
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

    public TestPlayerControllerBuilder AutoPassUntilStackEmpty()
    {
        return Enqueue((
            async (match, player, options) =>
            {
                if (!match.Match!.Stack.IsEmpty())
                    return (PassChoice(options), true, false);

                return (null, false, true);
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

    public TestPlayerControllerBuilder Tap(string name)
    {
        return Enqueue((
            async (match, player, options) =>
            {
                var permanent = match.Match!.Battlefield.GetPermanents().Single(p => p.HasName(name));
                permanent.Tapped.Set(true);
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

        
        public Asserts CanActivate()
        {
            options.Any(a => a.ToCommandString().StartsWith(ActivateAbilityAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} should be able to activate abilities"
            );
            return this;
        }

        public Asserts CanActivateMana()
        {
            options.Any(a => a.ToCommandString().StartsWith(ActivateManaAbilityAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} should be able to activate abilities"
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

        public Asserts CantActivate()
        {
            options.All(a => !a.ToCommandString().StartsWith(ActivateAbilityAction.ActionWord)).ShouldBeTrue(
                $"Player {player.GetDisplayName()} shouldn't be able to activate abilities"
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
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return ("Yes", true);
        });
    }

    public TestPlayerControllerBuilder No()
    {
        return Enqueue(async (player, options, hint, allowNone) =>
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
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (options[0], true);
        });
    }

    public TestPlayerControllerBuilder FirstWithName(string name)
    {
        return Enqueue(async (player, options, hint, allowNone) =>
        {
            return (options.First(c => c.HasName(name)), true);
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint, allowNone) =>
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

public class ManaPaymentChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.ManaPaymentChoice>(builder)
{
    public TestPlayerControllerBuilder NTimes(int n, Action<ManaPaymentChoicesBuilder> action)
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
            return (options.First(o => o is StoredManaPaymentChoice m && m.Mana.Type == type), true);
        });
    }

    public TestPlayerControllerBuilder ActivateFirst()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.First(o => o is ManaAbilityManaPaymentChoice), true);
        });
    }

    public TestPlayerControllerBuilder FirstStored()
    {
        return Enqueue(async (player, options, hint) =>
        {
            return (options.First(o => o is StoredManaPaymentChoice), true);
        });
    }


    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options, hint) =>
        {
            action(new(player, options, hint));
            return (null!, false);
        });
    }
    
    public class Asserts(Player player, IManaPaymentChoice[] options, string hint)
    {
        public Asserts NonNull()
        {
            player.ShouldNotBeNull();
            options.ShouldNotBeNull();
            hint.ShouldNotBeNull();
            return this;
        }

        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }
    }
}

public class AttackDeclarationsChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.AttackDeclarationsChoice>(builder)
{
    public TestPlayerControllerBuilder Skip()
    {
        return Enqueue(async (player, options) =>
        {
            return ([], true);
        });
    }

    private List<Func<AttackDeclaration[], AttackDeclaration>> _attackQueue = [];

    public AttackDeclarationsChoicesBuilder Player(string name, int idx)
    {
        _attackQueue.Add(options => options.Single(o => 
            o.Attacker.HasName(name) &&
            o.Target.GetTarget() == o.Attacker.Match.Players[idx]
        ));
        return this;
    }

    public TestPlayerControllerBuilder Done()
    {
        Func<AttackDeclaration[], AttackDeclaration>[] attacks = [.. _attackQueue];
        return Enqueue(async (player, options) =>
        {
            AttackDeclaration[] result = [.. attacks.Select(a => a(options))];
            return (result, true);
        });
    }
    
    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options) =>
        {
            action(new(player, options));
            return (null, false);
        });
    }

    public class Asserts(Player player, AttackDeclaration[] options)
    {
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }

        public Asserts CanAttackPlayer(int idx)
        {
            var target = player.Match.Players[idx];
            options.Any(o => o
                .Target.GetTarget() == target
            ).ShouldBeTrue();

            return this;
        }

        public Asserts CanAttackPlayer(string name, int idx)
        {
            var target = player.Match.Players[idx];
            options.Any(o => 
                o.Target.GetTarget() == target &&
                o.Attacker.HasName(name) 
            ).ShouldBeTrue();

            return this;
        }
    }
}

public class BlockDeclarationsChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.BlockDeclarationsChoice>(builder)
{
    private List<Func<BlockDeclaration[], BlockDeclaration>> _blockQueue = [];

    public BlockDeclarationsChoicesBuilder Block(string attackerName, string blockerName)
    {
        _blockQueue.Add(options => options.Single(o => 
            o.Blocker.HasName(blockerName) && // ! ????
            o.Attackers.Length == 1 &&
            o.Attackers[0].HasName(attackerName)
        ));
        return this;
    }

    public TestPlayerControllerBuilder Done()
    {
        Func<BlockDeclaration[], BlockDeclaration>[] Blocks = [.. _blockQueue];
        return Enqueue(async (player, options) =>
        {
            BlockDeclaration[] result = [.. Blocks.Select(a => a(options))];
            return (result, true);
        });
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options) =>
        {
            action(new(player, options));
            return (null, false);
        });
    }
    
    public class Asserts(Player player, BlockDeclaration[] options)
    {
        public Asserts OptionsCount(int v)
        {
            options.Length.ShouldBe(v);
            return this;
        }

        public Asserts CanBlock(string attackerName, string blockerName)
        {
            options.Any(o => 
                o.Attackers.Any(a => a.HasName(attackerName)) &&
                o.Blocker.HasName(blockerName)
            ).ShouldBeTrue();
            
            return this;
        }
    }
}
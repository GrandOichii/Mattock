using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Activated;


namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class CommandChoicesBuilder
    : ChoicesBuilder<(TestPlayerController.CommandChoice, bool)>
{
    public RollbackChoicesBuilder Rollback { get; }

    public CommandChoicesBuilder(TestPlayerControllerBuilder builder) : base(builder)
    {
        Rollback = new(this);
    }

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
            async (wrapper, player, options) => throw new IntentionalCrashException(),
            false
        ));
    }

    private static (ICommand, RollbackRequest?) PassChoice(ICommand[] options) =>
        (
            options.Single(o => o.ToCommandString() == PassAction.ActionWord),
            null
        );

    public TestPlayerControllerBuilder Pass()
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                return (PassChoice(options), true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder PlayLandWithName(string name)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                var land = player.GetPlayableLands().First(c => c.HasName(name));
                var command = new PlayLandCommand(land);
                return (Respond(command), true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder CastSpellWithName(string name)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                var card = player.GetCastableCards().First(c => c.HasName(name));
                var command = new CastSpellCommand(player, card);
                return (Respond(command), true, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder Activate(string permanentName, int abilityIdx = 0)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                ActivatedAbility[] arr = [.. player
                    .GetActivatableAbilities()
                    .Where(a => a.Card.HasName(permanentName))];

                var command = new ActivateAbilityCommand(player, arr[abilityIdx]);
                return (Respond(command), true, true);

            },
            true
        ));
    }

    public TestPlayerControllerBuilder ActivateMana(string permanentName, int abilityIdx = 0)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                ActivatedAbility[] arr = [.. player
                    .GetActivatableManaAbilities()
                    .Where(a => a.Card.HasName(permanentName))
                ];

                var command = new ActivateManaAbilityCommand(player, arr[abilityIdx]);
                return (Respond(command), true, true);

            },
            true
        ));
    }

    public TestPlayerControllerBuilder AddMana(ManaType type, int amount)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                player.ManaPool.AddGenericMana(type, amount);
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder SetPlayerStatus(int playerIdx, PlayerStatus status, bool silent = false)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                wrapper.GetMatch().Players[playerIdx].SetStatus(status, silent);
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder CheckForWinners()
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                wrapper.GetMatch().CheckForWinners();
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder AutoPass()
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                return (PassChoice(options), true, false);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassUntilStackEmpty()
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                if (!wrapper.GetMatch().Stack.IsEmpty())
                    return (PassChoice(options), true, false);

                return (RespondNull<ICommand>(), false, true);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToStep(StepType step)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                var currentStep = wrapper.GetMatch().TurnManager.Turn!.GetCurrentPhase().GetCurrentStep();
                if (currentStep is null || currentStep.Type != step)
                    return (PassChoice(options), true, false);
                return (RespondNull<ICommand>(), false, true);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToPhase(PhaseType phase)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                var currentPhase = wrapper.GetMatch().TurnManager.Turn!.GetCurrentPhase();
                if (currentPhase is null || currentPhase.Type != phase)
                    return (PassChoice(options), true, false);
                return (RespondNull<ICommand>(), false, true);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder AutoPassToTurn(int turn)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                if (wrapper.GetMatch().TurnManager.TurnCounter == turn)
                    return (RespondNull<ICommand>(), false, true);
                return (PassChoice(options), true, false);
            },
            false
        ));
    }

    public TestPlayerControllerBuilder SetLife(int playerIdx, int life)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                wrapper.GetMatch().Players[playerIdx].Life.Set(life);
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder Tap(string name)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                var permanent = wrapper.GetMatch().Battlefield.GetPermanents().Single(p => p.HasName(name));
                permanent.Tapped.Set(true);
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue((
            async (wrapper, player, options) =>
            {
                action(new(wrapper, player, options));
                return (RespondNull<ICommand>(), false, true);
            },
            true
        ));
    }

    public class Asserts(TestSessionWrapper wrapper, Player player, ICommand[] options)
    {
        public Asserts AssertMatch(Action<MatchAsserts> action)
        {
            action(new(wrapper));
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

    public class RollbackChoicesBuilder(CommandChoicesBuilder ccb)
    {
        public TestPlayerControllerBuilder ToTurn(int turn)
        {
            return ccb.Enqueue((
                async (wrapper, player, options) =>
                {
                    var id = wrapper.Session!.Snapshots.Snapshots.First(s => s.Meta.TurnCounter == turn).Id;
                    return (
                        (null, new() { RequestedSnapshotId = id} ),
                        true,
                        true
                    );
                },
                true
            ));
        }

        public TestPlayerControllerBuilder ToLast()
        {
            return ccb.Enqueue((
                async (wrapper, player, options) =>
                {
                    var id = wrapper.Session!.Snapshots.Snapshots.Last().Id;
                    return (
                        (null, new() { RequestedSnapshotId = id} ),
                        // (null, null),
                        true,
                        true
                    );
                },
                true
            ));
        }
    }
}


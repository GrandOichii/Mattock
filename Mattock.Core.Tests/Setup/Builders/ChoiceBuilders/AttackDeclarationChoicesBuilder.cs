namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class AttackDeclarationsChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.AttackDeclarationsChoice>(builder)
{
    public TestPlayerControllerBuilder Skip()
    {
        return Enqueue(async (player, options) =>
        {
            return (
                Respond<AttackDeclaration[]>([]),
                true
            );
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
            return (
                Respond(result),
                true
            );
        });
    }
    
    public TestPlayerControllerBuilder Assert(Action<Asserts> action)
    {
        return Enqueue(async (player, options) =>
        {
            action(new(player, options));
            return ((null, null), false);
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
namespace Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

public class BlockDeclarationsChoicesBuilder(TestPlayerControllerBuilder builder)
    : ChoicesBuilder<TestPlayerController.BlockDeclarationsChoice>(builder)
{
    private List<Func<BlockDeclaration[], BlockDeclaration>> _blockQueue = [];

    public BlockDeclarationsChoicesBuilder Block(string attackerName, string blockerName)
    {
        _blockQueue.Add(options => options.Single(o => 
            o.Blocker.HasName(blockerName) &&
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
    
    public class Asserts(Player player, BlockDeclaration[] options)
    {
        public Asserts NonNull()
        {
            player.ShouldNotBeNull();
            options.ShouldNotBeNull();
            return this;
        }

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

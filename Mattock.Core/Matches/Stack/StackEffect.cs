using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Matches.Stack;

public class StackEffect
{
    public Match Match { get; }
    public string Sid { get; }
    public IStackEffectResolver Resolver { get; }
    public Player? Controller { get; private set; }
    
    public StackEffect(TheStack stack, IStackEffectResolver resolver)
    {
        Match = stack.Match;
        Sid = stack.GenerateSid();
        Resolver = resolver;
        Controller = null;
    }

    public void SetController(Player player)
    {
        Controller = player;
    }

    public async Task Resolve()
    {
        await Resolver.Resolve(this);
    }

    public bool IsCard(Card card) => Resolver.IsCard(card);
}
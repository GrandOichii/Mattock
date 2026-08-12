using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Stack.Resolvers;

namespace Mattock.Core.Matches.Stack;

public class StackEffect(
    MatchStack stack,
    EffectContext ctx,
    IStackEffectResolver resolver
)
{
    public Match Match { get; } = stack.Match;
    public string Sid { get; } = stack.GenerateSid();
    public IStackEffectResolver Resolver { get; } = resolver;
    public EffectContext Ctx { get; } = ctx;

    public async Task Resolve()
    {
        await Resolver.Resolve(this);
    }

    public bool IsCard(Card card) => Resolver.IsCard(card);
}
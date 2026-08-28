using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Stack.Resolvers;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack;

public class MatchStack(
    Match match
) : ICardZone
{
    public Match Match = match;

    public List<StackEffect> Effects { get; } = [];

    public bool IsEmpty() => Effects.Count == 0;

    public int GetCount() => Effects.Count;

    public StackEffect? GetStackEffectByStackEffectIdid(string stackEffectId)
        => Effects.SingleOrDefault(e => e.StackEffectId == stackEffectId);

    public StackEffect Create(
        Card card,
        EffectContext ctx
    )
    {
        var stackEffectId = Match.MoveCard(
            card,
            CardZoneChangeType.Bottom,
            new SpellCardZoneChanger(
                ctx.Controller,
                ctx
            )
        );

        if (stackEffectId is null)
            throw new CodeErrorException($"Failed to move a card stack effect for card {card.GetDisplayName()}");

        var result = GetStackEffectByStackEffectIdid(stackEffectId);
        if (result is null)
            throw new CodeErrorException($"Failed to fetch newly created stack effect with StackEffectId = {stackEffectId} (cast card {card.GetDisplayName()})");

        return result;
    }

    public StackEffect Create(
        EffectContext ctx,
        IStackEffectResolver resolver
    )
    {
        var effect = new StackEffect(
            this,
            ctx,
            resolver
        );

        Effects.Add(effect);
        return effect;
    }

    public string GetZoneName() => "TheStack";

    public void Remove(Card card)
    {
        var idx = Effects.FindIndex(e => e.IsCard(card));
        Effects.RemoveAt(idx);
    }

    public async Task ResolveTop()
    {
        var top = Effects.Last();
        await top.Resolve();

        Effects.Remove(top);
    }

    class SpellCardZoneChanger(
        Player controller,
        EffectContext ctx
    ) : ICardZoneChanger
    {
        public bool Accepts(Card card)
        {
            // TODO
            return true;
        }

        public string Do(Card card, CardZoneChangeType type)
        {
            var stack = card.Match.Stack;

            if (type == CardZoneChangeType.Top)
                throw new CodeErrorException($"Tried to move {card.GetDisplayName()} to the top of the stack");
        
            var effect = new StackEffect(
                stack,
                ctx,
                new SpellResolver(card)
            );

            card.Match.Stack.Effects.Add(effect);
            return effect.StackEffectId;
        }

        public ICardZone GetTargetZone()
            => controller.Match.Stack;
    }
}
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Snapshots;
using Mattock.Core.Matches.Stack.Resolvers;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack;

public class MatchStack(
    Match match
) : ICardZone, IHasSnapshot<MatchStack.Snapshot>
{
    public Match Match = match;
    private int _lastId = 0;

    public List<StackEffect> Effects { get; } = [];

    public string GenerateSid() => $"se{++_lastId}";

    public bool IsEmpty() => Effects.Count == 0;

    public int GetCount() => Effects.Count;

    public StackEffect? GetStackEffectBySid(string sid) => Effects.SingleOrDefault(e => e.Sid == sid);

    public StackEffect Create(
        Card card,
        EffectContext ctx
    )
    {
        var sid = Match.MoveCard(
            card,
            CardZoneChangeType.Bottom,
            new SpellCardZoneChanger(
                ctx.Controller,
                ctx
            )
        );

        if (sid is null)
            throw new Exception($"Failed to move a card stack effect for card {card.GetDisplayName()}");

        var result = GetStackEffectBySid(sid);
        if (result is null)
            throw new Exception($"Failed to fetch newly created stack effect with SID = {sid} (cast card {card.GetDisplayName()})");

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

    public Snapshot GetSnapshot()
    {
        return new()
        {
            // TODO
        };
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
        throw new NotImplementedException();
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
                throw new Exception($"Tried to move {card.GetDisplayName()} to the top of the stack");
        
            var effect = new StackEffect(
                stack,
                ctx,
                new SpellResolver(card)
            );

            card.Match.Stack.Effects.Add(effect);
            return effect.Sid;
        }

        public ICardZone GetTargetZone()
            => controller.Match.Stack;
    }

    public class Snapshot
    {
        // TODO
    }
}
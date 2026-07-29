using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Matches.Stack.Resolvers;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Stack;

public class TheStack(Match match) : ICardZone
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
        Player controller,
        TargetDeclarationCollection targets
    )
    {
        var sid = Match.MoveCard(
            card,
            CardZoneChangeType.Bottom,
            new SpellCardZoneChanger(
                controller,
                targets
            )
        );

        if (sid is null)
            throw new Exception($"Failed to move a card stack effect for card {card.GetDisplayName()}");

        var result = GetStackEffectBySid(sid);
        if (result is null)
            throw new Exception($"Failed to fetch newly created stack effect with SID = {sid} (cast card {card.GetDisplayName()})");

        return result;
    }

    public string GetZoneName() => "TheStack";

    public void Remove(Card card)
    {
        var idx = Effects.FindIndex(e => e.IsCard(card));
        Effects.RemoveAt(idx);
    }

    // public string Add(
    //     Card card,
    //     CardZoneChangeType type,
    //     ICardZoneChangeContext ctx
    // )
    // {
    //     return ctx.Do();
    // }

    public async Task ResolveTop()
    {
        var top = Effects.Last();
        await top.Resolve();
    }

    class SpellCardZoneChanger(
        Player controller,
        TargetDeclarationCollection targets
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
                controller,
                new(
                    new SpellEffectContextData(
                        controller,
                        card
                    ),
                    targets
                ),
                new SpellResolver(card)
            );

            card.Match.Stack.Effects.Add(effect);
            return effect.Sid;
        }

        public ICardZone GetTargetZone()
            => controller.Match.Stack;
    }
}
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Context.Data;

public class SpellEffectContextData(
    Player owner,
    Card card
) : IEffectContextData
{
    public Player Owner { get; } = owner;

    // public LuaTable GetTable(Match match)
    // {
    //     return LuaCommon.CreateTable(match.LState, new()
    //     {
    //         { "owner", owner },
    //     });
    // }
}
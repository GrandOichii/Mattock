using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting;

public class DynamicManaCost
{
    public string Text { get; }
    public ManaType? Type { get; }
    public LuaFunction GetAmountFunc { get; }

    public DynamicManaCost(LuaTable table)
    {
        Text = LuaCommon.Get<string>(table, "Text");
        GetAmountFunc = LuaCommon.Get<LuaFunction>(table, "GetAmount");
        int type = LuaCommon.GetInt(table, "Type");
        Type = type == -1
            ? null
            : (ManaType)type;
    }

    public ManaCost ToManaCost(EffectContext ctx)
    {
        var returned = GetAmountFunc.Call(ctx);
        int amount = LuaCommon.GetReturnAsInt(returned);

        return new()
        {
            Amount = amount,
            Type = Type,
        };
    }
}
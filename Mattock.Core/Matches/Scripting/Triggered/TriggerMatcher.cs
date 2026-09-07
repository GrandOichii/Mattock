using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Triggers;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Triggered;

public class TriggerMatcher(
    LuaTable table
)
{
    public TriggerType Type { get; } = (TriggerType)LuaCommon.GetInt(table, "Type");
    public LuaFunction[] Filters { get; } = LuaCommon.ParseTable<LuaFunction>(
        LuaCommon.Get<LuaTable>(table, "Filters")
    );

    public bool CanTrigger(EffectContext ctx, Trigger trigger)
    {
        if (trigger.Type != Type) return false;

        foreach (var filter in Filters)
        {
            var returned = filter.Call(ctx, trigger.Ctx);
            if (!LuaCommon.GetReturnAsBool(returned))
                return false;
        }

        return true;
    }
}
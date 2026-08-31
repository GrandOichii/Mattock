using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Targets;

public class Target
{
    public LuaFunction GetFunc { get; }
    public LuaFunction CheckFunc { get; }

    public Target(LuaTable table)
    {
        GetFunc = LuaCommon.Get<LuaFunction>(table, "Get");
        CheckFunc = LuaCommon.Get<LuaFunction>(table, "Check");
    }

    public bool CanTarget(EffectContext ctx)
    {
        var returned = CheckFunc.Call(ctx);
        return LuaCommon.GetReturnAsBool(returned);
    }

    public (TargetDeclaration, RollbackRequest?) Get(EffectContext ctx)
    {
        var returned = GetFunc.Call(ctx);
        if (returned[0] is null)
        {
            var request = RollbackRequest.FromLuaReturned(returned, 1)
                ?? throw new CodeErrorException($"Target returned null target declaration and null rollback request");
            return (TargetDeclaration.ROLLBACK, request);
        }
        var table = LuaCommon.GetReturnAs<LuaTable>(returned);
        var key = LuaCommon.Get<string>(table, "Key");
        var list = LuaCommon.Get<LuaTable>(table, "Items");

        return (
            new(key, LuaCommon.ParseTable<object>(list)),
            null
        );
    }
}
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting;

public class Cost : ICost
{
    public string Text { get; }
    public LuaFunction PayFunc { get; }
    public LuaFunction CheckFunc { get; }

    public Cost(LuaTable table)
    {
        Text = LuaCommon.Get<string>(table, "Text");
        PayFunc = LuaCommon.Get<LuaFunction>(table, "Pay");
        CheckFunc = LuaCommon.Get<LuaFunction>(table, "Check");
    }

    public bool CanPay(EffectContext ctx)
    {
        var returned = CheckFunc.Call(ctx);
        return LuaCommon.GetReturnAsBool(returned);
    }

    public Task<RollbackRequest?> Pay(EffectContext ctx)
    {
        var returned = PayFunc.Call(ctx);
        if (returned[0] == null)
            return Task.FromResult<RollbackRequest?>(null);

        var request = LuaCommon.GetReturnAs<RollbackRequest>(returned);

        return Task.FromResult<RollbackRequest?>(request);
    }
}
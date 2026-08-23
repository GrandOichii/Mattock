using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using NLua;

namespace Mattock.Core.Matches.Scripting;

public class EffectPart(
    LuaFunction func
)
{
    public LuaFunction Func { get; } = func;

    public RollbackRequest? Do(EffectContext ctx)
    {
        var returned = Func.Call(ctx);

        return RollbackRequest.FromLuaReturned(returned);
    }
}
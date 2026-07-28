using Mattock.Core.Matches.Scripting.Context;
using NLua;

namespace Mattock.Core.Matches.Scripting;

public class EffectPart(
    LuaFunction func
)
{
    public LuaFunction Func { get; } = func;

    public void Do(EffectContext ctx)
    {
        Func.Call(ctx);
    }
}
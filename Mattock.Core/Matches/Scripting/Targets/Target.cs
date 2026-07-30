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

    
}
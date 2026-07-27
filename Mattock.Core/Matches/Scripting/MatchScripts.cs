using System.Linq.Expressions;
using System.Reflection;
using NLua;

namespace Mattock.Core.Matches.Scripting;


/// <summary>
/// Marks the method as a Lua function
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
internal class LuaCommand : Attribute { }

public class MatchScripts
{
    public Match Match { get; }
    public MatchScripts(Match match)
    {
        Match = match;

        // load all methods into the Lua state
        var type = typeof(MatchScripts);
        foreach (var method in type.GetMethods())
        {
            if (method.GetCustomAttribute<LuaCommand>() is not null)
            {
                Match.LState[method.Name] = method.CreateDelegate(Expression.GetDelegateType(
                    [.. from parameter in method.GetParameters() select parameter.ParameterType, method.ReturnType]
                ), this);
            }
        }
    }

    [LuaCommand]
    public void DrawCards(LuaTable playerTable, int amount)
    {
        // TODO
    }
}
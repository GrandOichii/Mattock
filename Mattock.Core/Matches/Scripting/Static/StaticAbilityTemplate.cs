using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Static;

public class StaticAbilityTemplate
{
    public string Text { get; }

    public StaticAbilityTemplate(LuaTable table)
    {
        Text = LuaCommon.Get<string>(table, "Text");
    }
}
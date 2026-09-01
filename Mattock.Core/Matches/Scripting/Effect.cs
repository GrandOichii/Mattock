using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting;

public class Effect
{
    public string Text { get; }
    public EffectPart[] Parts { get; }
    public Target[] Targets { get; }
    public bool CanProduceMana { get; }


    public Effect(LuaTable data)
    {
        Text = LuaCommon.Get<string>(data, "Text");
        CanProduceMana = LuaCommon.GetBool(data, "CanProduceMana");

        var effectsTable = LuaCommon.Get<LuaTable>(data, "Effects");
        var effects = LuaCommon.ParseTable<LuaFunction>(effectsTable);

        Parts = [.. effects.Select(f => new EffectPart(f))];

        var targetsTable = LuaCommon.Get<LuaTable>(data, "Targets");
        var targets = LuaCommon.ParseTable<LuaTable>(targetsTable);

        Targets = [.. targets.Select(t => new Target(t))];
    }

    public RollbackRequest? Do(EffectContext ctx)
    {
        try
        {
            foreach (var part in Parts)
            {
                var rollback = part.Do(ctx);
                if (rollback is not null)
                    return rollback;
            }
            return null;
        } catch (Exception e)
        {
            throw new ScriptingException($"Failed to execute effect with text \"{Text}\"", e);
        }
    }
}
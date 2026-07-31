using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Scripting.Activated;

public class ActivatedAbilityTemplate
{
    public string Text { get; }
    public Cost[] Costs { get; }
    public Effect[] Effects { get; }
    public DynamicManaCost[] ManaCosts { get; }

    public ActivatedAbilityTemplate(LuaTable table)
    {
        Text = LuaCommon.Get<string>(table, "Text");

        try
        {
            var effectsTable = LuaCommon.Get<LuaTable>(table, "Effects");
            var arr = LuaCommon.ParseTable<LuaTable>(effectsTable);
            Effects = [.. arr.Select(t => new Effect(t))];
        } catch (Exception e)
        {
            throw new Exception($"Failed to get effects for activated ability with text \"{Text}\"", e); // TODO type
        }

        try
        {
            var manaCostsTable = LuaCommon.Get<LuaTable>(table, "ManaCosts");
            var arr = LuaCommon.ParseTable<LuaTable>(manaCostsTable);
            ManaCosts = [.. arr.Select(t => new DynamicManaCost(t))];
        } catch (Exception e)
        {
            throw new Exception($"Failed to get costs for activated ability with text \"{Text}\"", e); // TODO type
        }

        try
        {
            var costsTable = LuaCommon.Get<LuaTable>(table, "Costs");
            var arr = LuaCommon.ParseTable<LuaTable>(costsTable);
            Costs = [.. arr.Select(t => new Cost(t))];
        } catch (Exception e)
        {
            throw new Exception($"Failed to get costs for activated ability with text \"{Text}\"", e); // TODO type
        }
    }
}
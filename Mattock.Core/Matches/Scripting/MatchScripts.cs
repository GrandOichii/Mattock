using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Utility;
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
    public void DEBUG(object o)
    {
        Debug.Print(o.ToString());
        // Match.Logger?.LogDebug(o.ToString());
    }

    [LuaCommand]
    public void DEBUGTABLE(LuaTable table)
    {
        Debug.Print(table.Keys.Count.ToString());
        // Match.Logger?.LogDebug(table.Keys.Count.ToString());
    }

    [LuaCommand]
    public void DrawCards(LuaTable playerTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playerTable);

        Match.Events.DrawCards([..
            players.Select(p => new CardDraw(p, amount))
        ]).Wait();
    }

    [LuaCommand]
    public void DiscardCards(LuaTable playerTable, int amount, bool random)
    {
        // TODO
    }

    [LuaCommand]
    public void GainLife(LuaTable playerTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playerTable);

        Match.Events.GainLife([..
            players.Select(p => new LifeGain(p, amount))
        ]).Wait();
    }

    [LuaCommand]
    public void LoseLife(LuaTable playerTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playerTable);

        Match.Events.LoseLife([..
            players.Select(p => new LifeLoss(p, amount))
        ]).Wait();
    }

    [LuaCommand]
    public LuaTable GetPlayersInAPNAP()
    {
        var result = Match.GetPlayersInAPNAP();
        return LuaCommon.CreateTable(Match.LState, result);
    }

    [LuaCommand]
    public LuaTable GetPermanents()
    {
        var result = Match.Battlefield.GetPermanents();
        return LuaCommon.CreateTable(Match.LState, result);
    }

    [LuaCommand]
    public Player GetPermanentController(Permanent permanent)
    {
        return permanent.GetController();
    } 

    [LuaCommand]
    public bool PermanentHasType(Permanent permanent, string type)
    {
        return permanent.HasType(type);
    }

    [LuaCommand]
    public LuaTable GetTargetDeclarationCollectionItems(TargetDeclarationCollection targets, string tgtKey)
    {
        var declaration = targets.Get(tgtKey);
        return LuaCommon.CreateTable(Match.LState, declaration.Items);
    }

    [LuaCommand]
    public LuaTable ChoosePlayers(Player player, LuaTable optionsTable, int min, int max, string hint)
    {
        var options = LuaCommon.ParseTable<Player>(optionsTable);
        var result = player.ChoosePlayers(options, min, max, hint)
            .GetAwaiter().GetResult();

        return LuaCommon.CreateTable(Match.LState, result);
    }
}
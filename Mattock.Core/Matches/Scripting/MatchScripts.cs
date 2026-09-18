using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Mattock.Core.Matches.Damage.Sources;
using Mattock.Core.Matches.Damage.Targets;
using Mattock.Core.Matches.Events;
using Mattock.Core.Matches.Mana;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Matches.Players.Mana;
using Mattock.Core.Matches.Rollback;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Matches.Zones;
using Mattock.Core.Setup;
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

    private LuaTable CreateResponseTable<T>((T[], RollbackRequest?) response)
    {
        return LuaCommon.CreateTable(Match.LState, new()
        {
            { "Response", LuaCommon.CreateTable(Match.LState, response.Item1) },
            { "Rollback", response.Item2 },
        });
    }

    private LuaTable CreateResponseTable<T>((T, RollbackRequest?) response)
    {
        return LuaCommon.CreateTable(Match.LState, new()
        {
            { "Response", response.Item1 },
            { "Rollback", response.Item2 },
        });
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
    public RollbackRequest? DrawCards(LuaTable playersTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playersTable);

        return Match.Events.DrawCards([..
            players.Select(p => new CardDraw(p, amount))
        ]).GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? Mill(LuaTable playersTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playersTable);

        return Match.Events.Mill([..
            players.Select(p => new Mill(p, amount))
        ]).GetAwaiter().GetResult();
    }


    [LuaCommand]
    public RollbackRequest? DiscardCards(LuaTable playersTable, int amount, bool random)
    {
        var players = LuaCommon.ParseTable<Player>(playersTable);

        return Match.Events.Discard([..
            players.Select(p => new Discard(p, amount, random))
        ]).GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? GainLife(LuaTable playerTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playerTable);

        return Match.Events.GainLife([..
            players.Select(p => new LifeGain(p, amount))
        ]).GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? LoseLife(LuaTable playerTable, int amount)
    {
        var players = LuaCommon.ParseTable<Player>(playerTable);

        return Match.Events.LoseLife([..
            players.Select(p => new LifeLoss(p, amount))
        ]).GetAwaiter().GetResult();
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
    public bool PermanentHasSubtype(Permanent permanent, string subtype)
    {
        return permanent.HasSubtype(subtype);
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

        return CreateResponseTable(result);
    }

    [LuaCommand]
    public LuaTable ChooseAnyTargets(Player player, LuaTable permanentsOptionsTable, LuaTable playersOptionsTable, int min, int max, string hint)
    {
        var permanentsOptions = LuaCommon.ParseTable<Permanent>(permanentsOptionsTable);
        var playersOptions = LuaCommon.ParseTable<Player>(playersOptionsTable);

        var result = player.ChooseAnyTargets(
            [
                .. playersOptions.Select(p => new PlayerAnyTargetChoice(p)),
                .. permanentsOptions.Select(p => new PermanentAnyTargetChoice(p)),
            ],
            min,
            max,
            hint
        ).GetAwaiter().GetResult();

        return CreateResponseTable(result);
    }

    [LuaCommand]
    public LuaTable ChooseString(Player player, LuaTable optionsTable, string hint)
    {
        var options = LuaCommon.ParseTable<string>(optionsTable);
        var result = player.ChooseString(options, hint)
            .GetAwaiter().GetResult();

        return CreateResponseTable(result);
    }

    [LuaCommand]
    public LuaTable ChoosePermanents(Player player, LuaTable optionsTable, int min, int max, string hint)
    {
        var options = LuaCommon.ParseTable<Permanent>(optionsTable);
        var result = player.ChoosePermanents(options, min, max, hint)
            .GetAwaiter().GetResult();

        return CreateResponseTable(result);
    }

    [LuaCommand]
    public Permanent? GetPermanentById(string id)
    {
        return Match.Battlefield.GetPermanentById(id);
    }

    [LuaCommand]
    public bool PermanentIsTapped(Permanent permanent)
    {
        return permanent.IsTapped();
    }

    [LuaCommand]
    public MatchConfig GetConfig()
    {
        return Match.Config;
    }

    [LuaCommand]
    public bool PermanentIsSummoningSick(Permanent permanent)
    {
        return permanent.HasSummoningSickness;
    }

    [LuaCommand]
    public RollbackRequest? TapPermanents(LuaTable arrTable)
    {
        Permanent[] permanents = LuaCommon.ParseTable<Permanent>(arrTable);
        return Match.Events.TapPermanents(permanents)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? DestroyPermanents(LuaTable arrTable)
    {
        Permanent[] permanents = LuaCommon.ParseTable<Permanent>(arrTable);
        return Match.Events.Destroy(permanents)
            .GetAwaiter().GetResult();
    }
    
    [LuaCommand]
    public RollbackRequest? UntapPermanents(LuaTable arrTable)
    {
        Permanent[] permanents = LuaCommon.ParseTable<Permanent>(arrTable);
        return Match.Events.UntapPermanents(permanents)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? DealDamageToPermanents(LuaTable damageTable)
    {
        var arr = LuaCommon.ParseTable<LuaTable>(damageTable);
        List<Damage.Damage> damages = [];
        foreach (var item in arr)
        {
            var permanent = LuaCommon.Get<Permanent>(item, "Permanent");
            var amount = LuaCommon.GetInt(item, "Amount");

            damages.Add(new(
                new TODODamageSource(amount),
                new PermanentDamageTarget(permanent)
            ));
        }
        return Match.Events.ProcessDamage([.. damages])
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? DealDamageToAnyTargets(LuaTable damageTable)
    {
        var arr = LuaCommon.ParseTable<LuaTable>(damageTable);
        List<Damage.Damage> damages = [];
        foreach (var item in arr)
        {
            var permanent = LuaCommon.Get<IAnyTargetChoice>(item, "AnyTarget");
            var amount = LuaCommon.GetInt(item, "Amount");

            damages.Add(new(
                new TODODamageSource(amount),
                new AnyTargetDamageTarget(permanent)
            ));
        }
        return Match.Events.ProcessDamage([.. damages])
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? DealDamageToPlayers(LuaTable damageTable)
    {
        var arr = LuaCommon.ParseTable<LuaTable>(damageTable);
        List<Damage.Damage> damages = [];
        foreach (var item in arr)
        {
            var player = LuaCommon.Get<Player>(item, "Player");
            var amount = LuaCommon.GetInt(item, "Amount");

            damages.Add(new(
                new TODODamageSource(amount),
                new PlayerDamageTarget(player)
            ));
        }
        return Match.Events.ProcessDamage([.. damages])
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public RollbackRequest? AddMana(LuaTable playersTable, LuaTable manaTable)
    {
        var players = LuaCommon.ParseTable<Player>(playersTable);
        var arr = LuaCommon.ParseTable<LuaTable>(manaTable);
        ManaAmount[] mana = [.. arr.Select(i => new ManaAmount(
            (ManaType)LuaCommon.GetInt(i, "Type"),
            LuaCommon.GetInt(i, "Amount")
        ))];

        return Match.Events.AddMana(players, mana)
            .GetAwaiter().GetResult();
    }

    [LuaCommand]
    public bool AreOpponents(Player p1, Player p2)
    {
        return p1.IsOpponentFor(p2);
    }

    [LuaCommand]
    public string GetZoneName(ICardZone zone)
    {
        return zone.GetZoneName();
    }

    [LuaCommand]
    public LuaTable GetZones()
    {
        var zones = Match.GetZones();
        return LuaCommon.CreateTable(Match.LState, zones);
    }

    [LuaCommand]
    public ICardZone GetCardZone(Card card)
    {
        return card.Zone;
    }

    [LuaCommand]
    public LuaTable GetCards()
    {
        var result = Match.GetCards();
        return LuaCommon.CreateTable(Match.LState, result);
    }

    [LuaCommand]
    public bool CardHasColor(Card card, int color)
    {
        return card.HasColor((Color)color);
    }
}
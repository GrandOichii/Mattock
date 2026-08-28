using Mattock.Core.Matches.Players.Costs;
using Mattock.Core.Matches.Scripting;
using Mattock.Core.Matches.Scripting.Activated;
using Mattock.Core.Matches.Scripting.Context;
using Mattock.Core.Matches.Scripting.Context.Data;
using Mattock.Core.Matches.Scripting.Targets;
using Mattock.Core.Matches.Zones;
using Mattock.Core.Setup.Templates;
using Mattock.Core.Utility;
using NLua;

namespace Mattock.Core.Matches.Players.Cards;

public class Card
{
    public Match Match { get; }
    public int OwnerIdx { get; }
    public string Id { get; }
    public CardTemplate Template { get; }
    public ICardZone? Zone { get; private set; }

    public Effect[] SpellEffects { get; }

    public ActivatedAbilityTemplate[] ActivatedAbilityTemplates { get; }
    public ActivatedAbility[] ActivatedAbilities { get; }

    public Card(Player owner, CardTemplate template)
    {
        Match = owner.Match;
        OwnerIdx = owner.Idx;
        Template = template;
        Zone = null;

        Id = Match.Ids.GenerateCardId(this);

        LuaTable data;
        try
        {
            Match.LState.DoString(template.Script);
            var creationFunc = LuaCommon.GetGlobalF(Match.LState, "_Create");
            var returned = creationFunc.Call();
            data = LuaCommon.GetReturnAs<LuaTable>(returned);
        }
        catch (Exception e)
        {
            throw new ScriptingException($"Failed to run card creation function in card {Template.Name}", e);
        }

        #region Spell effects

        try
        {
            var spellEffectsTable = LuaCommon.Get<LuaTable>(data, "SpellEffects");
            var arr = LuaCommon.ParseTable<LuaTable>(spellEffectsTable);
            SpellEffects = [.. arr.Select(t => new Effect(t))];
        } catch (Exception e)
        {
            throw new ScriptingException($"Failed to get spell effects for card {template.Name}", e);
        }

        #endregion

        #region Activated abilities

        try
        {
            var aaTable = LuaCommon.Get<LuaTable>(data, "ActivatedAbilities");
            var arr = LuaCommon.ParseTable<LuaTable>(aaTable);
            ActivatedAbilityTemplates = [.. arr.Select(t => new ActivatedAbilityTemplate(t))];

            // TODO might have to move this somewhere
            ActivatedAbilities = [.. ActivatedAbilityTemplates.Select(t => new ActivatedAbility(Match, t, this))];
        } catch (Exception e)
        {
            throw new ScriptingException($"Failed to get spell effects for card {template.Name}", e);
        }

        #endregion
    }

    public string GetShortName()
    {
        // TODO
        return Template.Name;
    }

    public string GetDisplayName() => $"{GetShortName()} {{{Id}}}"; // TODO

    public void SetZone(ICardZone zone)
    {
        Zone = zone;
    }

    public bool HasName(string name)
    {
        // TODO
        return Template.Name == name;
    }

    public bool HasType(string type)
    {
        // TODO
        return Template.Types.Contains(type);
    }

    public bool IsLand() => HasType("Land");

    public bool IsSorcery() => HasType("Sorcery");

    public bool IsInstant() => HasType("Instant");

    public bool IsPermanentType() => CardTypes.Permanents.Any(HasType);

    public ManaCost[] GetManaCosts(Player player)
    {
        // TODO
        return [ .. Template.ManaCosts ];
    }

    public bool CanBePlayedAsLand(Player player)
    {
        if (!IsLand()) return false;

        // TODO
        return Zone == player.Hand;
    }

    public bool CanBeCast(Player player)
    {
        if (IsLand())
            return false;

        if (!CardTypes.Castable.Any(HasType))
            return false;

        var ctx = new EffectContext(
            player,
            new SpellEffectContextData(
                // player,
                // this
            ),
            new([])
        );

        var costVariations = GetCostCollections(player);
        if (costVariations.All(c => !c.CanBePayed(ctx)))
            return false;

        // TODO this is very basic, change later
        if (!IsInstant() && !(Match.TurnManager.Turn!.GetCurrentPhase().IsMainPhase() && player.IsActive()))
            return false; 

        if (Zone != player.Hand)
            return false;

        var targets = GetSpellTargets();
        if (!targets.All(t => t.CanTarget(ctx)))
            return false;

        return true;
    }

    public Target[] GetSpellTargets()
    {
        return [.. SpellEffects.SelectMany(e => e.Targets)];
    }

    public List<CostCollection> GetCostCollections(Player player)
    {
        List<CostCollection> result = [];

        var manaCosts = GetManaCosts(player);
        if (manaCosts.Length > 0)
        {
            // TODO additional costs

            result.Add(new(
                "Default",
                [new ManaCostsCollection(manaCosts)]
            ));
        }
        // TODO alternative costs

        return result;
    }

    public async Task ResolveSpellEffects(EffectContext ctx)
    {
        foreach (var spellEffect in SpellEffects)
        {
            spellEffect.Do(ctx);
        }
    }

    public ActivatedAbility[] GetActivatableAbilitiesFor(Player player)
    {
        return [.. GetActivatedAbilities().Where(a => !a.IsManaAbility() && a.CanBeActivated(player))];
    }

    public ActivatedAbility[] GetActivatableManaAbilitiesFor(Player player)
    {
        return [.. GetActivatedAbilities().Where(a => a.IsManaAbility() && a.CanBeActivated(player))];
    }

    public ActivatedAbility[] GetActivatedAbilities()
    {
        // TODO
        return ActivatedAbilities;
    }
}
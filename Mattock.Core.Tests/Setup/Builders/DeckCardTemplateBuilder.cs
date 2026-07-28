namespace Mattock.Core.Tests.Setup.Builders;

public class DeckCardTemplateBuilder(string? cardName = null)
{
    public static readonly string DEFAULT_SCRIPT = """
    function _Create()
        return New:Card()
            :Build()
    end
    """;

    public readonly DeckCardTemplate _result = new()
    {
        Amount = 1,
        Card = new() {
            Name = cardName ?? $"c{++_lastCardId}",
            ColorIndicator = [],
            Defense = "",
            HandModifier = "",
            LifeModifier = "",
            Subtypes = [],
            Supertypes = [],
            Types = [],
            Loyalty = "",
            ManaCosts = [],
            Power = "",
            Toughness = "",
            TextBox = "",
            Script = DEFAULT_SCRIPT
        }
    };
    private static int _lastCardId = 0;

    public DeckCardTemplate Build() => _result;
    
    public DeckCardTemplateBuilder Amount(int v)
    {
        _result.Amount = v;
        return this;
    }

    public DeckCardTemplateBuilder Land() => AddType("Land");

    public DeckCardTemplateBuilder Artifact() => AddType("Artifact");

    public DeckCardTemplateBuilder Instant() => AddType("Instant");

    public DeckCardTemplateBuilder Sorcery() =>  AddType("Sorcery");

    public DeckCardTemplateBuilder AddType(string type)
    {
        _result.Card.Types = [.. _result.Card.Types, type];
        return this;
    }

    public DeckCardTemplateBuilder Power(string power)
    {
        _result.Card.Power = power;
        return this;
    }

    public DeckCardTemplateBuilder StatLine(string statline)
    {
        var stats = statline.Split("/");
        _result.Card.Power = stats[0];
        _result.Card.Toughness = stats[1];
        return this;
    }

    public DeckCardTemplateBuilder Toughness(string toughness)
    {
        _result.Card.Toughness = toughness;
        return this;
    }

    public DeckCardTemplateBuilder AddManaCost(ManaCost cost)
    {
        _result.Card.ManaCosts = [ .. _result.Card.ManaCosts, cost];
        return this;
    }

    public DeckCardTemplateBuilder ZeroCost() =>
        AddManaCost(new ManaCost()
        {
            Amount = 0,
            Type = null
        });

    public DeckCardTemplateBuilder ManaCost(ManaCost[] costs)
    {
        _result.Card.ManaCosts = costs;
        return this;
    }
}
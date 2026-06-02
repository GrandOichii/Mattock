namespace Mattock.Core.Tests.Setup.Builders;

public class DeckCardTemplateBuilder(string? cardName = null)
{
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
            Loyalty = 0,
            ManaCosts = [],
            Power = "",
            Toughness = "",
            TextBox = "",
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

    public DeckCardTemplateBuilder Instant() => AddType("Instant");

    public DeckCardTemplateBuilder Sorcery() =>  AddType("Sorcery");

    public DeckCardTemplateBuilder AddType(string type)
    {
        _result.Card.Types = [.. _result.Card.Types, type];
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
            Type = Matches.Mana.ManaType.Colorless
        });
}
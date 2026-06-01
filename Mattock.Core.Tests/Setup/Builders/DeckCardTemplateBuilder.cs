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
            ManaCost = [],
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

    public DeckCardTemplateBuilder Land()
    {
        return AddType("Land");
    }

    public DeckCardTemplateBuilder AddType(string type)
    {
        _result.Card.Types = [.. _result.Card.Types, type];
        return this;
    }
}
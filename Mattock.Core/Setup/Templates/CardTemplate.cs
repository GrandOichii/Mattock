using Mattock.Core.Matches;

namespace Mattock.Core.Setup.Templates;

public class CardTemplate
{
    public required string Name { get; set; }
    public required ManaCost[] ManaCost { get; set; }
    public required Color[] ColorIndicator { get; set; }

    public required string[] Types { get; set; }
    public required string[] Subtypes { get; set; }
    public required string[] Supertypes { get; set; }

    public required string TextBox { get; set; }
    public required string Power { get; set; }
    public required string Toughness { get; set; }
    public required int Loyalty { get; set; }
    public required string Defense { get; set; }
    public required string HandModifier { get; set; } // TODO type
    public required string LifeModifier { get; set; } // TODO type
}
using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public class StoredMana(
    ManaType type,
    string text = ""
)
{
    public ManaType Type { get; } = type;
    public string Text { get; set; } = text;

    public static StoredMana[] FromFormattedMana(string formattedMana)
    {
        var mana = ManaAmount.FromFormatted(formattedMana);
        
        return [
            .. ManaAmount.FromFormatted(formattedMana).SelectMany(m => Enumerable.Repeat(m.Type is null 
                ? new StoredMana(ManaType.Colorless)
                : new StoredMana((ManaType)m.Type)
            , m.Amount))
        ];
    }
}
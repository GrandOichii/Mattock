using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public class StoredMana
{
    public ManaType Type { get; }
    public string Text { get; set; }

    public StoredMana(ManaType type, string text = "")
    {
        Type = type;
        Text = text;
    }

    public static StoredMana[] FromFormattedMana(string formattedMana)
    {
        // return [ .. .Select(FromMana) ];

        var mana = Mana.FromFormatted(formattedMana);
        
        return [
            .. Mana.FromFormatted(formattedMana).SelectMany(m => Enumerable.Repeat(m.Type is null 
                ? new StoredMana(ManaType.Colorless)
                : new StoredMana((ManaType)m.Type)
            , m.Amount))
        ];
    }

    // public static StoredMana FromMana(Mana mana)
    // {
    //     return mana.Type is null 
    //         ? new StoredMana(ManaType.Colorless)
    //         : new StoredMana((ManaType)mana.Type);
    // }
}
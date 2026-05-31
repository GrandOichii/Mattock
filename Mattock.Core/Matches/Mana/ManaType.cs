namespace Mattock.Core.Matches.Mana;

public enum ManaType
{
    White = 0,
    Blue = 1,
    Black = 2,
    Red = 3,
    Green = 4,
    Colorless = 5,
}

public static class ManaTypeExtensions
{
    public static string ToManaSymbol(this ManaType t) => "{" + (t switch
    {
        ManaType.White => "W",
        ManaType.Blue => "U",
        ManaType.Black => "B",
        ManaType.Red => "R",
        ManaType.Green => "G",
        ManaType.Colorless => "C",
        _ => throw new Exception($"Unrecognized mana type: {t}"),
    }) + "}";
}
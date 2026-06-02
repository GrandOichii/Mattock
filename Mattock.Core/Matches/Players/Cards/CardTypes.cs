namespace Mattock.Core.Matches.Players.Cards;

public static class CardTypes
{
    public static readonly string Artifact = "Artifact";
    public static readonly string Creature = "Creature";
    public static readonly string Enchantment = "Enchantment";
    public static readonly string Instant = "Instant";
    public static readonly string Land = "Land";
    public static readonly string Planeswalker = "Planeswalker";
    public static readonly string Sorcery = "Sorcery";
    public static readonly string Kindred = "Kindred";
    public static readonly string Dungeon = "Dungeon";
    public static readonly string Battle = "Battle";
    public static readonly string Phenomenon = "Phenomenon";
    public static readonly string Vanguard = "Vanguard";
    public static readonly string Conspiracy = "Conspiracy";

    public static readonly string[] Castable = [
        Artifact,
        Creature,
        Enchantment,
        Instant,
        Planeswalker,
        Sorcery,
        Battle,
    ];
}
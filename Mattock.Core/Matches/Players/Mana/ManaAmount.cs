using System.Text.RegularExpressions;
using Mattock.Core.Matches.Mana;

namespace Mattock.Core.Matches.Players.Mana;

public partial class ManaAmount(
    ManaType? type,
    int amount
)
{
    public ManaType? Type { get; init; } = type;
    public int Amount { get; init; } = amount;

    public static List<ManaAmount> FromFormatted(string formatted)
    {
        var pattern = ManaSymbolPattern();
        Dictionary<string, ManaAmount> map = [];
        Dictionary<string, int> amounts = [];

        var matches = pattern.Matches(formatted);
        // List<Mana> result = [];
        int generic = 0;
        bool hasGeneric = false;
        
        for (int i = 0; i < matches.Count; ++i)
        {
            var m = matches[i];
            var v = m.Groups[1].Value;
            if (int.TryParse(v, out int g))
            {
                hasGeneric = true;
                generic += g;
                continue;
            }

            if (amounts.TryGetValue(m.Value, out int value))
            {
                amounts[m.Value] = ++value;
                continue;
            }

            var type = TypeFromSymbol(v);
            map[m.Value] = new(type, 0);
            amounts[m.Value] = 1;
        }

        return [
            .. hasGeneric 
                ? new ManaAmount[] { 
                    new(null, generic)
                }
                : [],
            .. map.Select(pair => new ManaAmount(pair.Value.Type, amounts[pair.Key]))  
        ];

        // return result;
    }

    private static ManaType TypeFromSymbol(string s) => s switch
    {
        "W" => ManaType.White,
        "U" => ManaType.Blue,
        "B" => ManaType.Black,
        "R" => ManaType.Red,
        "G" => ManaType.Green,
        "C" => ManaType.Colorless,
        _ => throw new Exception($"Unrecognizable mana type symbol: {s}"),
    };

    [GeneratedRegex(@"\{(.+?)\}")]
    private static partial Regex ManaSymbolPattern();
}
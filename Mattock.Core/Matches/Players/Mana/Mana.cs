using System.Text.RegularExpressions;
using Mattock.Core.Matches.Mana;
using Microsoft.VisualBasic;

namespace Mattock.Core.Matches.Players.Mana;

public partial class Mana
{
    public required ManaType? Type { get; init; }
    public required int Amount { get; init; }

    public static List<Mana> FromFormatted(string formatted)
    {
        var pattern = ManaSymbolPattern();
        Dictionary<string, Mana> map = [];
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
            map[m.Value] = new()
            {
                Amount = 0,
                Type = type
            };
            amounts[m.Value] = 1;
        }

        return [
            .. hasGeneric 
                ? new Mana[] { 
                    new() {
                        Amount = generic,
                        Type = null
                    }
                }
                : [],
            .. map.Select(pair => new Mana() {
                Amount = amounts[pair.Key],
                Type = pair.Value.Type
            })  
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
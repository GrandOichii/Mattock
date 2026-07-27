using System.Text.Json;
using Mattock.Core.Setup.Templates;

namespace Mattock.Core.Loaders;

public class FileCardLoader : ICardLoader
{
    private static readonly string MANIFEST_FILE = "manifest.json";

    private static readonly string EXPANSION_MANIFEST_FILE = "manifest.json";

    private Dictionary<string, Dictionary<string, CardTemplate>> _expansionMap;

    public FileCardLoader(string dir)
    {
        var manifestPath = Path.Join(dir, MANIFEST_FILE);
        var data = JsonSerializer.Deserialize<ManifestData>(File.ReadAllText(manifestPath))
            ?? throw new Exception($"Null JSON at {manifestPath}");

        _expansionMap = [];
        foreach (var expansion in data.Expansions)
        {
            Dictionary<string, CardTemplate> cards = [];

            var expansionDir = Path.Join(dir, expansion); // cards/M10
            var expansionManifestPath = Path.Join(expansionDir, EXPANSION_MANIFEST_FILE);
            var expansionData = JsonSerializer.Deserialize<ExpansionManifestData>(File.ReadAllText(expansionManifestPath))
                ?? throw new Exception($"Null JSON at {expansionManifestPath}");

            foreach (var card in expansionData.Cards)
            {
                var cardPath = Path.Join(expansionDir, card);
                var cardData = JsonSerializer.Deserialize<CardTemplate>(File.ReadAllText($"{cardPath}.json"))
                    ?? throw new Exception($"Null JSON at {cardPath}");
                var scriptPath = $"{cardPath}.lua";
                cardData.Script = File.ReadAllText(scriptPath);

                cards[cardData.Name] = cardData;
            }
            _expansionMap[expansion] = cards;
        }
    }
    
    public static string GetCardID(string name, string expansion)
        => $"{expansion}:{name}";

    public static (string expansion, string name) SplitCardID(string id)
    {
        var split = id.Split(":");
        if (split.Length != 2)
            throw new Exception($"Incorrect card id format for {nameof(FileCardLoader)}: {id}");

        return (split[0], split[1]);
    }

    public CardTemplate Load(string id)
    {
        var (expansion, name) = SplitCardID(id);
        if (!_expansionMap.TryGetValue(expansion, out var cardMap))
            throw new Exception($"Unrecognized expansion: {expansion} (for card {id})");
        if (!cardMap.TryGetValue(name, out var result))
            throw new Exception($"Unrecognized card in expansion {expansion}: {name} (for card {id})");
        return result;
    }
}

class ManifestData
{
    public required string[] Expansions { get; init; }
}

class ExpansionManifestData
{
    public required string[] Cards { get; init; }
}
using Mattock.Core.Scripts;

namespace Mattock.Core.Loaders;

public interface ICardScriptLoader
{
    string Load(string expansion, string cardName);
}

public class LuaCardScriptLoader(
    string dir
) : ICardScriptLoader
{
    public string Load(string expansion, string cardName)
    {
        return File.ReadAllText(
            Path.Join(dir, expansion, $"{cardName}.lua")
        );
    }
}

public class JsonCardScriptLoader(
    string dir
) : ICardScriptLoader
{
    public string Load(string expansion, string cardName)
    {
        var path = Path.Join(dir, expansion, $"{cardName}.script.json");
        if (!File.Exists(path))
        {
            return $"error('no json card script for {expansion}:{cardName}')";
        }
        return ScriptLoader.Load(path);
    }
}
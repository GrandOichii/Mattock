namespace Mattock.Core.Matches.Scripting.Targets;

public class TargetDeclaration(
    string key,
    object[] items
)
{
    public string Key { get; } = key;
    public object[] Items { get; } = items;
}
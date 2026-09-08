namespace ScriptEditor.Core.SimpleArgs;

/// <summary>
/// Enum argument (<c>Convert</c> expects a string value which has a mapped value in <c>Values</c>)
/// </summary>
public class EnumArgConfig : SimpleArgConfig
{
    public required Dictionary<string, string> Values { get; set; }

    public override (string, bool) Convert(object value)
    {
        if (value is not string label)
            throw new Exception($"Provided non-int value for {nameof(EnumArgConfig)}");

        return (Values[label], false);
    }
}
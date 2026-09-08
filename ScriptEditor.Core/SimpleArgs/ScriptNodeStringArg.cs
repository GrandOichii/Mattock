namespace ScriptEditor.Core.SimpleArgs;

/// <summary>
/// String argument (<c>Convert</c> expects a string value)
/// </summary>
public partial class StringArgConfig : SimpleArgConfig
{
    /// <summary>
    /// Default value (displayed on initialization)
    /// </summary>
    public required string Default { get; set; }

    /// <summary>
    /// Placeholder value
    /// </summary>
    public required string Placeholder { get; set; }

    /// <summary>
    /// If true, the editor should be multiline
    /// </summary>
    public required bool Multiline { get; set; }

    public override (string, bool) Convert(object value)
    {
        if (value is not string text)
            throw new Exception($"Provided non-string value for {nameof(StringArgConfig)}");

        return (text.Replace("\n", "\\n"), string.IsNullOrEmpty(text));
    }
}
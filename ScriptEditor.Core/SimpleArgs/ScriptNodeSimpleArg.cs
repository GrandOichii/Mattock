namespace ScriptEditor.Core.SimpleArgs;

/// <summary>
/// Simple argument of a script node
/// </summary>
public class ScriptNodeSimpleArg
{
    /// <summary>
    /// Key of the input (generated script will replace all instances of $key in output script)
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Prefix added to the generated script
    /// </summary>
    public required string Prefix { get; set; }

    /// <summary>
    /// Postfix added to the generated script
    /// </summary>
    public required string Postfix { get; set; } 

    // TODO docs
    public required SimpleArgConfig Config { get; set; }

    /// <summary>
    /// If true and the input is empty, an empty string will be returned on script generation (this ignores Prefix and Postfix)
    /// </summary>
    public bool NoScriptIfEmpty { get; set; } = false;

    public virtual string Convert(object value)
    {
        var (result, isEmpty) = Config.Convert(value);
        if (isEmpty && NoScriptIfEmpty) return "";

        return $"{Prefix}{result}{Postfix}";
    }
}

public abstract class SimpleArgConfig
{
    /// <summary>
    /// Convert the value from the UI element into a generated script
    /// </summary>
    /// <param name="value">Value from the UI element</param>
    /// <returns>Generated script</returns>
    public abstract (string, bool) Convert(object value);
}
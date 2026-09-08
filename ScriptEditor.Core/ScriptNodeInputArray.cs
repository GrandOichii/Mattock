namespace ScriptEditor.Core;

/// <summary>
/// Script node input port array
/// </summary>
public partial class ScriptNodeInputArray
{
    /// <summary>
    /// Input key
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Input type
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// Text displayed on the button that adds new ports
    /// </summary>
    public required string AddButtonLabel { get; set; }

    /// <summary>
    /// Prefix added to the final generated script
    /// </summary>
    public required string ScriptPrefix { get; set; }

    /// <summary>
    /// Postfix added to the final generated script
    /// </summary>
    public required string ScriptPostfix { get; set; }

    /// <summary>
    /// Prefix added to each individual generated script
    /// </summary>
    public required string ItemPrefix { get; set; }

    /// <summary>
    /// Postfix added to each individual generated script
    /// </summary>
    public required string ItemPostfix { get; set; }

    /// <summary>
    /// If true and no ports are connected, the generated script will be an empty string (this ignores ScriptPrefix and ScriptPostfix)
    /// </summary>
    public required bool NoScriptIfEmpty { get; set; }

    /// <summary>
    /// Separator inserted between each item
    /// </summary>
    public required string ItemSeparator { get; set; }

    /// <summary>
    /// Position of the ports
    /// </summary>
    public required ScriptNodePortPosition Position { get; set; }
}
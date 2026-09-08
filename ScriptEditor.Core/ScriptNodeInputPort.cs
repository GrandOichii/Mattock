namespace ScriptEditor.Core;

/// <summary>
/// Script node input port
/// </summary>
public partial class ScriptNodeInputPort 
    : ScriptNodePort
{
    /// <summary>
    /// Key of the input (generated script will replace all instances of $key in output script)
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Generated script prefix
    /// </summary>
    public required string Prefix { get; set; }

    /// <summary>
    /// Generated script postfix
    /// </summary>
    public required string Postfix { get; set; }

    /// <summary>
    /// If true and the port is not connected to anything, MissingScript will be displayed
    /// </summary>
    public required bool HasMissingScript { get; set; } = false;

    /// <summary>
    /// Will be displayed if HasMissingScript = true and the port is not connected to anything
    /// </summary>
    public required string MissingScript { get; set; }

    /// <summary>
    /// If true multiple connections can be made to the port
    /// </summary>
    public required bool AllowMultiple { get; set; }

    /// <summary>
    /// Separator inserted between each generated text (if AllowMultiple = true)
    /// </summary>
    public required string MultipleSeparator { get; set; }
}
namespace ScriptEditor.Core;

/// <summary>
/// Position of script node
/// </summary>
public enum ScriptNodePortPosition
{
    /// <summary>
    /// Left port
    /// </summary>
    Left,

    /// <summary>
    /// Right port
    /// </summary>
    Right,
}

/// <summary>
/// Script node port
/// </summary>
public class ScriptNodePort
{
    /// <summary>
    /// Label displayed near the port
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// Position of the port
    /// </summary>
    public required ScriptNodePortPosition Position { get; set; }

    /// <summary>
    /// Type of the port
    /// </summary>
    public required string Type { get; set; }
}
namespace ScriptEditor.Core;

/// <summary>
/// Script node output port
/// </summary>
public class ScriptNodeOutputPort : ScriptNodePort
{
    /// <summary>
    /// Generated script
    /// </summary>
    public required string Script { get; set; }
}
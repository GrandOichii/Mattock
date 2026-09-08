using ScriptEditor.Core.SimpleArgs;

namespace ScriptEditor.Core;

/// <summary>
/// Script node
/// </summary>
public class ScriptNode
{
    /// <summary>
    /// Unique script node name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Label, displayed on top of the script node
    /// </summary>
    public required string Label { get; set; }

    /// <summary>
    /// Input ports
    /// </summary>
    public required List<ScriptNodeInputPort> Inputs { get; set; }
    
    /// <summary>
    /// Output ports
    /// </summary>
    public required List<ScriptNodeOutputPort> Outputs { get; set; }

    /// <summary>
    /// Input array
    /// </summary>
    public required ScriptNodeInputArray? InputArray { get; set; }

    /// <summary>
    /// Simple arguments
    /// </summary>
    public required List<ScriptNodeSimpleArg> SimpleArgs { get; set; }

    /// <summary>
    /// Script node description
    /// </summary>
    public required string Description { get; set; }
}
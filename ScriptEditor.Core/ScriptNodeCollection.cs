namespace ScriptEditor.Core;

/// <summary>
/// Collection of script nodes
/// </summary>
public partial class ScriptNodeCollection
{
    /// <summary>
    /// List of script nodes
    /// </summary>
    public required List<ScriptNode> Nodes { get; set; }

    // public required Dictionary<string, string> AllowedConnections { get; set; }

}
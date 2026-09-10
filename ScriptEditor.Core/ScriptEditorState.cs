using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace ScriptEditor.Core;

/// <summary>
/// State of the editor
/// </summary>
public class ScriptEditorState
{
    /// <summary>
    /// Id of the start node
    /// </summary>
    public required int StartId { get; init; }

    /// <summary>
    /// Node states
    /// </summary>
    public required List<ScriptNodeState> Nodes { get; init; }

    /// <summary>
    /// Node connections
    /// </summary>
    public required List<ScriptNodeConnection> Connections { get; init; }

    /// <summary>
    /// Generate a script
    /// </summary>
    /// <param name="scriptNodes">Script node definitions</param>
    /// <returns>The generated script</returns>
    public string GenerateScript(ScriptNodeCollection scriptNodes)
    {
        // find start
        ScriptNodeState start = Nodes.FirstOrDefault(n => n.Id == StartId)
            ?? throw new System.Exception("Start node not found");

        Dictionary<string, ScriptNode> mapping = scriptNodes.Nodes.ToDictionary(n => n.Name);
        
        return start.Generate(0, this, mapping);
    }
}

/// <summary>
/// State of a script node
/// </summary>
public class ScriptNodeState
{
    /// <summary>
    /// Unique Id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Name of the script node
    /// </summary>
    public required string Name { get; set; }

    // TODO all editor properties should probably be removed
    /// <summary>
    /// X offset in editor
    /// </summary>
    public required float EditorOffsetX { get; set; }

    /// <summary>
    /// Y offset in editor
    /// </summary>
    public required float EditorOffsetY { get; set; }

    /// <summary>
    /// Width of the node in the editor
    /// </summary>
    public float EditorSizeX { get; set; }

    /// <summary>
    /// Height of the node in the editor
    /// </summary>
    public float EditorSizeY { get; set; }

    /// <summary>
    /// State of the input array
    /// </summary>
    public required ScriptNodeInputArrayState? InputArray { get; set; }

    /// <summary>
    /// States for simple arguments
    /// </summary>
    public required Dictionary<string, object> Data { get; set; }

    // TODO add docs
    public string Generate(
        int outputIdx,
        ScriptEditorState parent,
        Dictionary<string, ScriptNode> mapping
    )
    {
        var node = mapping.GetValueOrDefault(Name)
            ?? throw new System.Exception($"Script node not found in mapping: {Name}");
        bool isStart = Id == parent.StartId;
        var output = node.Outputs[outputIdx];

        int leftIdx = isStart ? 0 : node.Outputs.Count(o => o.Position == ScriptNodePortPosition.Left);
        int rightIdx = isStart ? 0 : node.Outputs.Count(o => o.Position == ScriptNodePortPosition.Right);

        Dictionary<string, string> valueMap = [];

        // regular inputs
        foreach (var input in node.Inputs)
        {
            List<ScriptNodeConnection> cs;

            if (input.Position == ScriptNodePortPosition.Left)
            {
                cs = [.. parent.Connections.Where(c => 
                    c.ToId == Id &&
                    c.ToPort == leftIdx
                )];
                ++leftIdx;
            } else
            {
                cs = [.. parent.Connections.Where(c => 
                    c.FromId == Id &&
                    c.FromPort == rightIdx
                )];
                ++rightIdx;
            }

            if (!input.AllowMultiple && cs.Count == 0)
            {
                var value = "!missing connection!";
                if (input.HasMissingScript)
                {
                    value = input.MissingScript;
                }
                valueMap[input.Key] = value;
                continue;
            }

            List<string> values = [];
            foreach (var connection in cs)
            {
                string value = "ERR";
                if (input.Position == ScriptNodePortPosition.Left)
                {
                    var from = parent.Nodes.FirstOrDefault(n => n.Id == connection.FromId)
                        ?? throw new System.Exception($"Node with Id = {connection.FromId} not found");
                    value = from.Generate(connection.FromPort, parent, mapping);
                } else
                {
                    var to = parent.Nodes.FirstOrDefault(n => n.Id == connection.ToId)
                        ?? throw new System.Exception($"Node with Id = {connection.ToId} not found");
                    value = to.Generate(connection.ToPort, parent, mapping);
                }
                value = $"{input.Prefix}{value}{input.Postfix}";
                values.Add(value);
            }
            valueMap[input.Key] = string.Join(input.MultipleSeparator, values);
        }

        // input array
        if (InputArray is not null && node.InputArray is not null)
        {
            List<string> inputArrayValues = [];
            for (int i = 0; i < InputArray.SlotCount; ++i)
            {
                
                if (node.InputArray.Position == ScriptNodePortPosition.Left)
                {
                    var port = leftIdx + i;
                    var connection = parent.Connections.SingleOrDefault(c => 
                        c.ToId == Id &&
                        c.ToPort == port
                    );
                    ++leftIdx;
                    if (connection is null) continue;

                    var from = parent.Nodes.FirstOrDefault(n => n.Id == connection.FromId)
                        ?? throw new System.Exception($"Node with Id = {connection.FromId} not found");
                    var value = from.Generate(connection.FromPort, parent, mapping);
                    inputArrayValues.Add($"{node.InputArray.ItemPrefix}{value}{node.InputArray.ItemPostfix}");
                    
                } else
                {
                    var port = rightIdx + i;
                    var connection = parent.Connections.SingleOrDefault(c => 
                        c.FromId == Id &&
                        c.FromPort == port
                    );
                    ++rightIdx;
                    if (connection is null) continue;

                    var from = parent.Nodes.FirstOrDefault(n => n.Id == connection.ToId)
                        ?? throw new System.Exception($"Node with Id = {connection.ToId} not found");
                    var value = from.Generate(connection.ToPort, parent, mapping);
                    inputArrayValues.Add($"{node.InputArray.ItemPrefix}{value}{node.InputArray.ItemPostfix}");
                    
                }
            }
            var joined = "";
            if (inputArrayValues.Count > 0 || !node.InputArray.NoScriptIfEmpty)
            {
                joined = $"{node.InputArray.ScriptPrefix}{string.Join(node.InputArray.ItemSeparator, inputArrayValues)}{node.InputArray.ScriptPostfix}";
            }
            valueMap[node.InputArray.Key] = joined;
        }

        // simple args
        foreach (var arg in node.SimpleArgs)
        {
            var value = Data.GetValueOrDefault(arg.Key)
                ?? throw new Exception($"Value not found for key {arg.Key} for node with Id = {Id}");
            
            valueMap[arg.Key] = arg.Convert(value);
        }

        var result = output.Script;
        foreach (var (key, value) in valueMap)
        {
            result = result.Replace($"${key}", value);
        }

        return result;
    }

}

/// <summary>
/// State of the input array
/// </summary>
public class ScriptNodeInputArrayState
{
    /// <summary>
    /// Number of slots in the array
    /// </summary>
    public required int SlotCount { get; set; }
}

/// <summary>
/// Script node connection
/// </summary>
public class ScriptNodeConnection
{
    /// <summary>
    /// Id of the node from which the connection is made (left end)
    /// </summary>
    public required int FromId { get; set; }

    /// <summary>
    /// Source port
    /// </summary>
    public required int FromPort { get; set; }

    /// <summary>
    /// Id of the node to which the connection is maded (кшпре end)
    /// </summary>
    public required int ToId { get; set; }

    /// <summary>
    /// Target port
    /// </summary>
    public required int ToPort { get; set; }
}
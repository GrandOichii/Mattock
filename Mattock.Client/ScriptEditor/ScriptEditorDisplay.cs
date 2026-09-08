
using ScriptEditor.Core;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


/// <summary>
/// Script editor
/// </summary>
public partial class ScriptEditorDisplay : Control
{
    [Signal]
    public delegate void StateChangedEventHandler();

    [Export]
    public PackedScene ScriptNodeDisplayScene { get; set; }

    [Export]
    public Godot.Collections.Dictionary<string, Color> ObjectColors { get; set; }

	private ScriptNodeCollection _scriptNodes;

    #region Nodes

    public GraphEdit GraphNode { get; private set; }
    public RichTextLabel GeneratedScriptDisplayNode { get; private set; }
    public AddNewNodeWindow AddNewNodeWindowNode { get; private set; }

    #endregion

    public ScriptEditorState State { get; private set; }

    private Dictionary<string, int> _typeMap;
    private Dictionary<string, ScriptNode> _nodeMap;

    public override void _Ready()
    {
        #region Nodes 

        GraphNode = GetNode<GraphEdit>("%Graph");
        GeneratedScriptDisplayNode = GetNode<RichTextLabel>("%GeneratedScriptDisplay");
        AddNewNodeWindowNode = GetNode<AddNewNodeWindow>("%AddNewNodeWindow");

        #endregion
    }

    public void LoadScriptNodes(ScriptNodeCollection scriptNodes)
    {
        AddNewNodeWindowNode.LoadScriptNodes(scriptNodes);
        _scriptNodes = scriptNodes;
        
        // load node and type map
        var typeSet = new HashSet<string>();
        _typeMap = [];
        int typeId = -1;

        _nodeMap = [];
        foreach (var node in _scriptNodes.Nodes)
        {
            _nodeMap[node.Name] = node;
            List<string> types = [
                .. node.Outputs.Select(p => p.Type),
                .. node.Inputs.Select(p => p.Type),
                node.InputArray?.Type
            ];
            foreach (var type in types)
            {
                if (type is null) continue;
                if (typeSet.Contains(type)) continue;
                typeSet.Add(type);
                _typeMap[type] = ++typeId;                
            }
        }
    }

    private ScriptNodeDisplay CreateScriptNode(ScriptNodeState state, bool isStart)
    {
        var scriptNode = _nodeMap.GetValueOrDefault(state.Name)
            ?? throw new Exception($"ScriptNode not found: {state.Name}");

        var result = ScriptNodeDisplayScene.Instantiate() as ScriptNodeDisplay;
        result.SetEssentials(this);
        GraphNode.AddChild(result);

        result.LoadScriptNodeState(scriptNode, state, isStart);
        result.Show();
        return result;
    }
    
    /// <summary>
    /// Load script editor state
    /// </summary>
    /// <param name="state">Script editor state</param>
    public void LoadState(ScriptEditorState state)
    {
        GraphNode.ClearConnections();
        foreach (var node in GraphNode.GetChildren())
        {
            if (node.Name == "_connection_layer") continue;
			var child = node as ScriptNodeDisplay;
            GraphNode.RemoveChild(child);
            child.QueueFree();
        }
        
        State = state;

        Dictionary<int, ScriptNodeDisplay> displayMap = [];
        foreach (var node in state.Nodes)
        {
            var display = CreateScriptNode(node, node.Id == state.StartId);
            displayMap[node.Id] = display;
        }

        // foreach (var display in displayMap.Values)
        //     display.PrintPorts();

        foreach (var connection in state.Connections)
        {
            var from = displayMap[connection.FromId];			
			var to = displayMap[connection.ToId];

			GraphNode.ConnectNode(
				from.Name,
				connection.FromPort,
				to.Name,
				connection.ToPort
			);	
        }

        RegenerateScript();
    }

    public void UpdateState()
    {
        var lastId = -1;
        Dictionary<string, int> nodeNameToIdMap = [];

        // nodes
        List<ScriptNodeState> nodeStates = []; 
        int startId = -1;
        foreach (var node in GraphNode.GetChildren())
        {
            if (node.Name == "_connection_layer") continue;
			var child = node as ScriptNodeDisplay;

            var id = ++lastId;
            var displayState = child.ToState(id);
            nodeStates.Add(displayState);
            nodeNameToIdMap[child.Name] = id;

            if (!child.IsStart) continue;
            if (startId != -1)
                throw new Exception($"Two start nodes found while updating state");
            startId = id;
        }

        // connections
        List<ScriptNodeConnection> connections = [];
        foreach (var connection in GraphNode.Connections)
		{
			var fromNode = connection["from_node"].AsString();
			var fromPort = connection["from_port"].AsInt32();
			var toNode = connection["to_node"].AsString();
			var toPort = connection["to_port"].AsInt32();

			var fromId = nodeNameToIdMap[fromNode];
			var toId = nodeNameToIdMap[toNode];

			connections.Add(new()
			{
				FromId = fromId,
				FromPort = fromPort,
				ToId = toId,
				ToPort = toPort
			});
		}

        State = new ScriptEditorState()
        {
            Nodes = nodeStates,
            Connections = connections,
            StartId = startId
        };

        // script
        RegenerateScript();

        EmitSignalStateChanged();
    }

    private void RegenerateScript()
    {
        var script = GenerateScript();
        GeneratedScriptDisplayNode.Clear();
        GeneratedScriptDisplayNode.AppendText($"-- {DateTime.Now}\n\n");
        GeneratedScriptDisplayNode.AppendText(script);
    }

    private List<Connection> GetConnections()
    {
        var connectionsRaw = GraphNode.Connections;
        List<Connection> result = [];
        foreach (var connection in connectionsRaw)
        {
            var toNode = connection["to_node"].AsString();
            var fromNode = connection["from_node"].AsString();
            var toSlot = connection["to_port"].AsInt32();
            var fromSlot = connection["from_port"].AsInt32();

            var from = GraphNode.GetNode<ScriptNodeDisplay>(fromNode);
            var to = GraphNode.GetNode<ScriptNodeDisplay>(toNode);

            result.Add(new()
            {
                From = from,
                FromSlot = fromSlot,
                To = to,
                ToSlot = toSlot,
            });
        }

        return result;
    }

    private string GenerateScript()
    {
        ScriptNodeDisplay start = null;
        foreach (var node in GraphNode.GetChildren())
        {
            if (node.Name == "_connection_layer") continue;
			var child = node as ScriptNodeDisplay;

            if (!child.IsStart) continue;
            if (start is not null)
                throw new Exception("Found two starts while generating script");
            
            start = child;
        }

        if (start is null)
        {
            throw new Exception($"Start node not found");
        }


        return Regex.Unescape(
            State.GenerateScript(_scriptNodes)
        );
    }

    public int GetSlotType(string type)
    {
        if (!_typeMap.TryGetValue(type, out int value))
        {
            throw new Exception($"Unrecognized slot type: {type}");
        }

        return value;
    }

    public Color GetSlotColor(string type)
    {
        if (!ObjectColors.TryGetValue(type, out Color value))
        {
            throw new Exception($"Unrecognized slot type: {type}");
        }

        return value;
    }

	private Vector2 _lastGraphMousePos;
    private void AddScriptNodeToMouseLocation()
	{
		_lastGraphMousePos = (GraphNode.GetLocalMousePosition() + GraphNode.ScrollOffset) / GraphNode.Zoom;
		// ResetAddNewNodeWindow();
        AddNewNodeWindowNode.Reset();
		AddNewNodeWindowNode.Reveal();
	}

    private void AddScriptNode(
        string name,
        Vector2 pos
    )
    {
        CreateScriptNode(
            new()
            {
                EditorOffsetX = pos.X,
                EditorOffsetY = pos.Y,
                EditorSizeX = 0,
                EditorSizeY = 0,
                Id = -1,
                Name = name,
                InputArray = null,
                Data = []
            },
            false
        );
    }

    private void TryDelete(string nodeName)
    {
        var connections = GraphNode.GetConnectionListFromNode(nodeName);
		if (connections.Count > 0)
		{
			return;
		}
		var node = GraphNode.GetNode(nodeName);
		GraphNode.RemoveChild(node);
		node.Free();

        UpdateState();
    }

    #region Signal connections

    public void OnGraphGuiInput(InputEvent e)
	{
		// if (!_editable) return;
		if (e.IsActionPressed("add_script_node"))
		{
			AddScriptNodeToMouseLocation();
		}
	}

    public void OnGraphConnectionRequest(string fromNode, int fromSlot, string toNode, int toSlot)
	{
		// if (!_editable) return;
        if (fromNode == toNode) return;

        // TODO detect cyclical connections
        
        var connections = GetConnections();

        var from = GraphNode.GetNode<ScriptNodeDisplay>(fromNode);
        var to = GraphNode.GetNode<ScriptNodeDisplay>(toNode);

        List<(ScriptNodeDisplay, int, ScriptNodePortPosition, Func<Connection, bool> predicate)> args = [
            (
                from,
                fromSlot,
                ScriptNodePortPosition.Right,
                c => c.From == from && c.FromSlot == fromSlot
            ),
            (
                to,
                toSlot,
                ScriptNodePortPosition.Left,
                c => c.To == to && c.ToSlot == toSlot
            ),
        ];

        foreach (var (display, port, pos, predicate) in args)
        {
            List<Connection> cs = [.. connections.Where(predicate)];
            if (cs.Count > 0 && !display.CanHaveMultipleConnections(port, pos))
            {
                foreach (var c in cs)
                {
                    GraphNode.DisconnectNode(c.From.Name, c.FromSlot, c.To.Name, c.ToSlot);
                }
            }
        }


		GraphNode.ConnectNode(fromNode, fromSlot, toNode, toSlot);
        UpdateState();
	}

    public void OnGraphDisconnectionRequest(string fromNode, int fromSlot, string toNode, int toSlot)
	{
		// if (!_editable) return;
		GraphNode.DisconnectNode(fromNode, fromSlot, toNode, toSlot);
		UpdateState();
	}

    public void OnGraphEndNodeMove()
    {
        UpdateState();
    }

    public void OnAddNewNodeWindowScriptNodeChosen(string scriptNodeName)
    {
        AddScriptNode(scriptNodeName, _lastGraphMousePos);
        UpdateState();
    }

    public void OnGraphDeleteNodesRequest(Godot.Collections.Array<string> nodeNames)
    {
        // if (!_editable) return;

        // TODO change this so that if none of the deleted nodes are connected to non-deleted nodes the deletion can happen
		foreach (var name in nodeNames)
		{
			TryDelete(name);
		}
    }

    public void OnGraphDuplicateNodesRequest()
    {
        CopyNodesToBuffer();
        PasteNodesFromBuffer();
    }

    public void OnGraphCopyNodesRequest()
    {
        CopyNodesToBuffer();
    }

    public void OnGraphPasteNodesRequest()
    {
        PasteNodesFromBuffer();
    }

    #endregion

    private List<ScriptNodeState> _buffer = [];
    private void CopyNodesToBuffer()
    {
        var duplicationXOffset = 30;
        var duplicationYOffset = 30;

        _buffer = [];
        foreach (var node in GraphNode.GetChildren())
        {
            if (node.Name == "_connection_layer") continue;
			var child = node as ScriptNodeDisplay;
            if (!child.Selected || child.IsStart) continue;

            var state = child.ToState(-1);
            state.EditorOffsetX += duplicationXOffset;
            state.EditorOffsetY += duplicationYOffset;
            
            _buffer.Add(state);
        }
    }

    private void PasteNodesFromBuffer()
    {
        if (_buffer.Count == 0) return;

        foreach (var st in _buffer)
        {
            CreateScriptNode(st, false);
        }

        UpdateState();
    }
}

public class Connection
{
    public required ScriptNodeDisplay From { get; init; }
    public required int FromSlot { get; init; }
    public required ScriptNodeDisplay To { get; init; }
    public required int ToSlot { get; init; }
}
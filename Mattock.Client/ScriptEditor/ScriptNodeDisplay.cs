using Godot;
using ScriptEditor.Core;
using ScriptEditor.Core.SimpleArgs;
using System;
using System.Collections.Generic;

public class Port<T>
{
    public required ScriptNodePortPosition Position { get; init; }
    public required int SlotIdx { get; init; }
    public required T Value { get; init; }
}

/// <summary>
/// Display for a script node
/// </summary>
public partial class ScriptNodeDisplay : GraphNode
{
	[Export]
	public PackedScene SlotScene { get; set; }

	public ScriptEditorDisplay Editor { get; private set; }

    private Dictionary<string, ISimpleArgHandler> _simpleArgHandlers;

	private ScriptNode _node;
    public ScriptNode Node => _node;

    public bool IsStart { get; private set; }

    public List<Port<ScriptNodeInputPort>> InputPorts { get; private set; }
    public List<Port<ScriptNodeInputArray>> InputArrayPorts { get; private set; }
    public List<Port<ScriptNodeOutputPort>> OutputPorts { get; private set; }

    public override void _Ready()
    {
        _simpleArgHandlers = [];
    }

	public void SetEssentials(ScriptEditorDisplay editor)
	{
		Editor = editor;
	}

    public int Left { get; set; }

    public int Right { get; set; }

	public void LoadScriptNode(ScriptNode node, bool isStart)
	{
		_node = node;
        IsStart = isStart;
        InputPorts = [];
        OutputPorts = [];
        InputArrayPorts = [];

		Title = node.Label;

		List<ScriptNodePort> ports = [
			.. isStart ? [] : (node.Outputs ?? []),
			// .. node.Outputs ?? [],
			.. node.Inputs ?? [],
		];

		var left = 0;
		var right = 0;

        var inputs = node.Inputs ?? [];
        var outputs = isStart ? [] : (node.Outputs ?? []);

        Left = 0;
        Right = 0;

		// standart ports
		foreach (var port in ports)
		{
            int loc;
			if (port.Position == ScriptNodePortPosition.Left)
			{
				++left;
                loc = left;
			}
            else
			{
				++right;
                loc = right;
			}
            
            CorrectSlots(loc);
            ConfigSlot(
                port.Position,
                loc - 1,
                port.Type
            );
            SetSlotLabel(port.Label, loc - 1, port.Position);
			RegisterPort(port);
		}

		var inputArrayStart = Math.Max(left, right) - 1;

		// input array
		if (node.InputArray is not null)
		{
			var btn = new Button
			{
				Text = node.InputArray.AddButtonLabel
			};
            btn.Pressed += OnAddArrSlotPressed;

            _inputArrayCount = inputArrayStart;

            AddArrSlot();
            _arrSlotCount = 1;
			AddChild(btn);
		}

		// simple args
        foreach (var simpleArg in node.SimpleArgs ?? [])
        {
            var el = ISimpleArgHandler.Create(simpleArg, this);
            AddHandler(simpleArg.Key, el);
        }
	}

    private int CalculateSlot(ScriptNodePort port)
    {
        int result;
        if (port.Position == ScriptNodePortPosition.Left)
        {
            result = Left;
            Left++;
        } else
        {
            result = Right;
            Right++;            
        }
        return result;
    }

    private void RegisterPort(ScriptNodePort port)
    {
        int pos = CalculateSlot(port);
        // TODO dont like this
        switch (port)
        {
            case ScriptNodeInputPort input:
                InputPorts.Add(new()
                {
                    Position = input.Position,
                    SlotIdx = pos,
                    Value = input
                });
                break;

            case ScriptNodeOutputPort output:
                OutputPorts.Add(new()
                {
                    Position = output.Position,
                    SlotIdx = pos,
                    Value = output
                });
                break;
        }
    }

    public void LoadScriptNodeState(ScriptNode node, ScriptNodeState state, bool isStart)
    {
        PositionOffset = new(state.EditorOffsetX, state.EditorOffsetY);
        Size = new(state.EditorSizeX, state.EditorSizeY);
        LoadScriptNode(node, isStart);

        LoadData(state.Data);

        if (node.InputArray is null) return;

        LoadInputArray(state.InputArray ?? new()
        {
            SlotCount = 1
        });
    }

    public void PrintPorts()
    {
        GD.Print($"Ports for {_node.Name}");

        GD.Print("\tInput:");
        foreach (var ip in InputPorts)
        {
            GD.Print($"\t\t{ip.Position}[{ip.SlotIdx}] -> {ip.Value.Key}");
        }
        if (_node.InputArray is not null)
        {
            GD.Print("\tInput array:");
            foreach (var ip in InputArrayPorts)
            {
                GD.Print($"\t\t{ip.Position}[{ip.SlotIdx}] -> {ip.Value.Key}");
            }
        }
        
        GD.Print("\tOutput:");
        foreach (var ip in OutputPorts)
        {
            GD.Print($"\t\t{ip.Position}[{ip.SlotIdx}] -> {ip.Value.Label}");
        }
        
    }

    private void OnAddArrSlotPressed()
    {
        AddArrSlot();
    }

    private int _inputArrayCount;
    private Control _lastArrSlot;
    private int _arrSlotCount;
    private void AddArrSlot()
    {
        // var label = new ColorRect()
        // {
        //     Color = new(1, 1, 0),
        //     CustomMinimumSize = new(0, 20)
        // };
        var label = new Label();

        if (_lastArrSlot is null)
        {
            AddChild(label);
        } else
        {
            _lastArrSlot.AddSibling(label);            
        }

        _lastArrSlot = label;
        ++_arrSlotCount; 

        ConfigSlot(
            _node.InputArray.Position,
            ++_inputArrayCount,
            _node.InputArray.Type
        );

        InputArrayPorts.Add(new()
        {
            Position = _node.InputArray.Position,
            SlotIdx = Left,
            Value = _node.InputArray
        });
        Left++;
    }

    private void ConfigSlot(
        ScriptNodePortPosition position,
        int idx,
        string type
    )
    {
        if (position == ScriptNodePortPosition.Left)
        {
            SetSlotEnabledLeft(idx, true);
            SetSlotTypeLeft(idx, Editor.GetSlotType(type));
            SetSlotColorLeft(idx, Editor.GetSlotColor(type));
            return;
        }

        SetSlotEnabledRight(idx, true);
        SetSlotTypeRight(idx, Editor.GetSlotType(type));
        SetSlotColorRight(idx, Editor.GetSlotColor(type));
    }

	private void SetSlotLabel(string label, int idx, ScriptNodePortPosition pos)
	{
		var slot = GetChild<ScriptNodeDisplaySlot>(idx);
		var node = pos switch
		{
			ScriptNodePortPosition.Left => slot.LeftLabelNode,
			ScriptNodePortPosition.Right => slot.RightLabelNode,
			_ => throw new Exception($"Unrecognized script node port position: {pos}")
		};

		node.Text = label;
	}

	private void CorrectSlots(int target)
	{
		while (GetChildCount() < target)
		{
			var child = SlotScene.Instantiate() as ScriptNodeDisplaySlot;

			AddChild(child);
		}
	}

	public void LoadData(Dictionary<string, object> data)
	{
        foreach (var (key, value) in data)
        {
            if (!_simpleArgHandlers.TryGetValue(key, out ISimpleArgHandler handler))
            {
                throw new Exception($"Simple arg handler for key {key} not found for node {_node.Name}");
            }

            handler.Load(value);
        }
	}

    public void AddHandler(string key, ISimpleArgHandler handler)
    {
        _simpleArgHandlers[key] = handler;
    }

    public void LoadInputArray(ScriptNodeInputArrayState state)
    {
        for (int i = 0; i < state.SlotCount - 1; ++i)
        {
            AddArrSlot();
        }
    }

    public ScriptNodeState ToState(int id)
    {
        return new()
        {
            Id = id,
            EditorOffsetX = PositionOffset.X,
            EditorOffsetY = PositionOffset.Y,
            EditorSizeX = Size.X,
            EditorSizeY = Size.Y,
            Name = _node.Name,
            InputArray = GetInputArrayState(),
            Data = GetSimpleArgData(),
        };
    }

    private ScriptNodeInputArrayState GetInputArrayState()
    {
        if (_node.InputArray is null) return null;

        return new()
        {
            SlotCount = _arrSlotCount
        };
    }

    private Dictionary<string, object> GetSimpleArgData()
    {
        Dictionary<string, object> result = [];
        foreach (var (key, handler) in _simpleArgHandlers)
        {
            result.Add(key, handler.GetValue());
        }
        return result;
    }

    public bool CanHaveMultipleConnections(int port, ScriptNodePortPosition pos)
    {
        foreach (var input in InputPorts)
        {
            if (input.Value.Position != pos) continue;
            --port;
            if (port >= 0) continue;
            return input.Value.AllowMultiple;
        }

        // is an output port
        return true;
    }

    #region Signal connections

    public void OnResizeEnd(Vector2 _)
    {
        Editor.UpdateState();
    }

    #endregion
}

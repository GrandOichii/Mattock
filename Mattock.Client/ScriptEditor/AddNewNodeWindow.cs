using Godot;
using ScriptEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AddNewNodeWindow : Window
{
    #region Signals

    [Signal]
    public delegate void ScriptNodeChosenEventHandler(string name);

    #endregion

    #region Nodes

    public Container NodeInfoContainerNode { get; private set; }
    public Tree TableNode { get; private set; }
    public Label NodeNameNode { get; private set; }
    public RichTextLabel NodeDescriptionNode { get; private set; }
    public LineEdit FilterEditNode { get; private set; }

    #endregion

    public override void _Ready()
    {
        #region Nodes

        NodeInfoContainerNode = GetNode<Container>("%NodeInfoContainer");
        TableNode = GetNode<Tree>("%Table");
        NodeNameNode = GetNode<Label>("%NodeName");
        NodeDescriptionNode = GetNode<RichTextLabel>("%NodeDescription");
        FilterEditNode = GetNode<LineEdit>("%FilterEdit");

        #endregion

        TableNode.SetColumnTitle(0, "Name");
        TableNode.SetColumnTitle(1, "Outputs");
        TableNode.SetColumnTitle(2, "Inputs");
    }

    public void Reveal()
    {
        FilterEditNode.GrabFocus();
        Show();
    }

    private Dictionary<string, ScriptNode> _nodeMap;

    private List<TreeItem> _items;

    public void LoadScriptNodes(ScriptNodeCollection scriptNodes)
    {
        TableNode.Clear();

        var root = TableNode.CreateItem();

        _nodeMap = [];
        Dictionary<string, List<ScriptNode>> categoryMap = [];
        foreach (var node in scriptNodes.Nodes)
        {
            _nodeMap[node.Name] = node;

            foreach (var output in node.Outputs)
            {
                if (!categoryMap.TryGetValue(output.Type, out List<ScriptNode> value))
                {
                    value = [];

                    categoryMap[output.Type] = value;
                }

                value.Add(node);
            }
        }

        _items = [];

        foreach (var (category, nodes) in categoryMap)
        {
            var categoryItem = root.CreateChild();
            categoryItem.SetText(0, category);

            foreach (var node in nodes)
            {
                var nodeItem = categoryItem.CreateChild();
                nodeItem.SetText(0, node.Label);
                nodeItem.SetText(1, $"[{string.Join(", ", node.Outputs.Select(o => o.Type))}]");
                nodeItem.SetText(2, $"[{string.Join(", ", node.Inputs.Select(i => i.Type))}]");
                nodeItem.SetMetadata(0, node.Name);
                _items.Add(nodeItem);
            }
        }

        ApplyFilter();

        // NodeListNode.Clear();
        // _scriptNodes = scriptNodes;

        // foreach (var node in scriptNodes.Nodes)
        // {
        //     if (
        //         !node.Name.Contains(FilterEditNode.Text, StringComparison.CurrentCultureIgnoreCase) &&
        //         !node.Label.Contains(FilterEditNode.Text, StringComparison.CurrentCultureIgnoreCase)
        //     ) continue;
                
        //     var idx = NodeListNode.AddItem(node.Label);
        //     NodeListNode.SetItemMetadata(idx, node.Name);
        // }
    }

    private void ApplyFilter()
    {
        foreach (var item in _items)
        {
            item.Visible = false;

            // TODO optimize
            if (item.GetText(0).Contains(FilterEditNode.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                item.Visible = true;
                continue;
            }
            if (item.GetText(1).Contains(FilterEditNode.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                item.Visible = true;
                continue;
            }
            if (item.GetText(2).Contains(FilterEditNode.Text, StringComparison.CurrentCultureIgnoreCase))
            {
                item.Visible = true;
                continue;
            }
        }
    }

    public void Reset()
    {
        FilterEditNode.Text = "";
        OnFilterEditTextChanged("");
        NodeInfoContainerNode.Hide();
    }

    private void ConfirmAdd()
    {
        var selected = TableNode.GetSelected();
        var nodeName = selected.GetMetadata(0).AsString();
        Hide();
        
        EmitSignalScriptNodeChosen(nodeName);
    }

    #region Signal connections

    public void OnNodeListItemSelected(int idx)
    {
        // TODO rename and reconnect
        // TODO bad
        // var item = _scriptNodes.Nodes[idx];

        // NodeNameNode.Text = item.Label;
        // NodeDescriptionNode.Text = item.Description;
        // NodeInfoContainerNode.Show();
    }

    public void OnTableItemActivated()
    {
        ConfirmAdd();
    }

    public void OnCloseRequested()
    {
        Hide();
    }

    public void OnCancelAddNewNodeButtonPressed()
    {
        Hide();
    }

    public void OnAddNewNodeButtonPressed()
    {
        ConfirmAdd();
    }

    public void OnFilterEditTextChanged(string _) {
        ApplyFilter();
        // LoadScriptNodes(_scriptNodes);
    }

    #endregion
}

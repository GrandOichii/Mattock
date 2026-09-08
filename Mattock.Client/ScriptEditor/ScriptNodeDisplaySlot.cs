using Godot;
using System;

public partial class ScriptNodeDisplaySlot : HBoxContainer
{
    #region Nodes

    public Label LeftLabelNode { get; set; }
    public Label RightLabelNode { get; set; }

    #endregion

    public override void _Ready()
    {
        #region Nodes

        LeftLabelNode = GetNode<Label>("%Left");
        RightLabelNode = GetNode<Label>("%Right");

        #endregion
    }

}

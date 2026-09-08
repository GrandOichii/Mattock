namespace ScriptEditor.Core.SimpleArgs;

/// <summary>
/// String Boolean (<c>Convert</c> expects a bool value)
/// </summary>
public class BoolArgConfig : SimpleArgConfig
{
    public required string TrueScript { get; set; }

    public required string FalseScript { get; set; }

    public required string Label { get; set; }

    public bool Default { get; set; } = false;

    // public override void Create(ScriptNodeDisplay display)
    // {
    //     var checkbox = new CheckBox()
    //     {
    //         Text = Label,
    //         ButtonPressed = Default
    //     };
    //     checkbox.Pressed += display.Editor.UpdateState;

    //     display.AddHandler(
    //         Key,
    //         new BoolArgHandler(this, checkbox)
    //     );

    //     display.AddChild(checkbox);
    // }

    public override (string, bool) Convert(object value)
    {
        if (value is not bool v)
            throw new System.Exception($"Provided non-bool value for {nameof(BoolArgConfig)}");

        return (v ? TrueScript : FalseScript, false);
    }
}

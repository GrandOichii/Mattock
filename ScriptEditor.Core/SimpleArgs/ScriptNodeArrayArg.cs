namespace ScriptEditor.Core.SimpleArgs;

public class ScriptNodeArrayArg  : ScriptNodeSimpleArg
{
    public required string ItemPrefix { get; set; }
    public required string ItemPostfix { get; set; }
    public required string Separator { get; set; }
    public required string AddButtonText { get; set; }

    public override string Convert(object value)
    {
        if (value is not List<object> list)
            throw new Exception($"Providede a non-list value for {nameof(ScriptNodeArrayArg)} (actual type: {value.GetType()})");

        List<string> results = [];
        foreach (var item in list)
            results.Add($"{ItemPrefix}{base.Convert(item)}{ItemPostfix}");
        
        return string.Join(Separator, results);
    }
}

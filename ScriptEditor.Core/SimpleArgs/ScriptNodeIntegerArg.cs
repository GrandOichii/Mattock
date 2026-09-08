namespace ScriptEditor.Core.SimpleArgs;

/// <summary>
/// Integer argument (<c>Convert</c> expects a long value)
/// </summary>
public partial class IntegerArgConfig : SimpleArgConfig
{
    public string Label { get; set; } = "";

    public required bool HasMin { get; set; }

    public required int Min { get; set; }

    public required bool HasMax { get; set; }

    public required int Max { get; set; }

    public int Default { get; set; } = 0;

    public bool ZeroMeansEmpty { get; set; } = false;

    public override (string, bool) Convert(object value)
    {
        if (value is long intV)
        {
            if (intV == 0 && ZeroMeansEmpty) return ("", true);
            return (intV.ToString(), false);
        }
        if (value is int longV)
        {
            if (longV == 0 && ZeroMeansEmpty) return ("", true);
            return (longV.ToString(), false);
        }

        throw new Exception($"Provided non-number value for {nameof(IntegerArgConfig)} (actual type: {value.GetType()})");
    }
}
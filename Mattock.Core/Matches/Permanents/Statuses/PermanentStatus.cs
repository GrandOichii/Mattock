namespace Mattock.Core.Matches.Permanents.Statuses;

public enum PermanentStatusType
{
    Tapped,
    Flipped,
    FaceUp,
    PhasedIn,
}

public class PermanentStatus(PermanentStatusType type, bool defaultV)
{
    public PermanentStatusType Type { get; } = type;
    public bool Value { get; private set; } = defaultV;
}
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches;


/// <summary>
/// Thrown during construction of class <c>Match</c> if a session with the specified parameters can't be created
/// </summary>
[Serializable]
public class CantStartException : Exception
{
    public CantStartException() { }
    public CantStartException(string message) : base(message) { }
    public CantStartException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if a team has more players than <c>MatchConfig.MaxTeamSize</c>
/// </summary>
[Serializable]
public class TeamTooBigException : CantStartException
{
    public TeamTooBigException() { }
    public TeamTooBigException(string message) : base(message) { }
    public TeamTooBigException(string message, System.Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if two or more players have duplicate names
/// </summary>
[Serializable]
public class DuplicatePlayerNameException : CantStartException
{
    public DuplicatePlayerNameException() { }
    public DuplicatePlayerNameException(string message) : base(message) { }
    public DuplicatePlayerNameException(string message, System.Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown during construction of class <c>Match</c> if the team count is bigger than <c>MatchConfig.TeamCount</c>
/// </summary>
[Serializable]
public class TooManyTeamsException : CantStartException
{
    public TooManyTeamsException() { }
    public TooManyTeamsException(string message) : base(message) { }
    public TooManyTeamsException(string message, System.Exception inner) : base(message, inner) { }
}

[System.Serializable]
public class UnhandledRollbackException(Player player, RollbackRequest rollback)
    : Exception($"Unhandled rollback from player {player.GetDisplayName()} to snapshot {rollback.RequestedSnapshotId}")
{
}
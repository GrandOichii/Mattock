using Mattock.Core.Matches.Players;

namespace Mattock.Core.Matches.Rollback;

public class RollbackRequest
{
    public required string RequestedSnapshotId { get; init; }

    public async Task<bool> IsApprovedByAll(Player initiator)
    {
        Player[] players = [.. initiator.Match.Players.Where(p => p != initiator)];
        string hint = $"Approve rollback to snapshot with id {RequestedSnapshotId} by player {initiator.GetDisplayName()}?"; // TODO better msg
        Task<bool>[] approvalTasks = [.. players.Select(p => p.ApproveRollback(hint))]; 
        var results = await Task.WhenAll(approvalTasks);
        return results.All(r => r);
    }
}
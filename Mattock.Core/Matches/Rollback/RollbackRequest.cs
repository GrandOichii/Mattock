using Mattock.Core.Matches.Players;
using Mattock.Core.Utility;

namespace Mattock.Core.Matches.Rollback;

public class RollbackRequest
{
    public readonly static RollbackRequest PLAYBACK_ROLLBACK = new() { RequestedSnapshotId = -1 };

    public required int RequestedSnapshotId { get; init; }

    public async Task<bool> IsApprovedByAll(Player initiator)
    {
        Player[] players = [.. initiator.Match.Players.Where(p => p != initiator)];
        string hint = $"Approve rollback to snapshot with id {RequestedSnapshotId} by player {initiator.GetDisplayName()}?"; // TODO better msg
        Task<bool>[] approvalTasks = [.. players.Select(p => p.ApproveRollback(hint))]; 
        var results = await Task.WhenAll(approvalTasks);
        return results.All(r => r);
    }

    public static RollbackRequest? FromLuaReturned(object[] returned, int idx=0)
    {
         if (returned[idx] == null)
            return null;

        return LuaCommon.GetReturnAs<RollbackRequest>(returned, idx);
    }
}
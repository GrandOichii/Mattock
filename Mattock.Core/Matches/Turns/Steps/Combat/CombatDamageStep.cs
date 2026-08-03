using Mattock.Core.Matches.Damage;
using Mattock.Core.Matches.Damage.Sources;
using Mattock.Core.Matches.Damage.Targets;
using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Turns.Phases;

namespace Mattock.Core.Matches.Turns.Steps.Combat;

public class CombatDamageStep(
    Phase phase
) : Step(phase, StepType.CombatDamage, true)
{
    public override bool CanBeTaken()
    {
        return Match.Battlefield.GetAttackingPermanents().Length > 0;
    }

    public override Task DoPostPriority()
    {
        // 510.4. (first strike)
        // TODO 
        return Task.CompletedTask;
    }

    public override async Task DoPrePriority()
    {
        // 510.1.
        var assignments = await AssignDamage();

        // 510.2.
        await Match.Events.ProcessDamage(assignments);
    }

    private async Task<Damage.Damage[]> AssignDamage()
    {
        var attackers = Match.Battlefield.GetAttackingPermanents();

        List<Damage.Damage> result = [];
        Dictionary<Permanent, List<Permanent>> blockMap = [];
        foreach (var attacker in attackers)
        {
            // * 510.1a
            if (attacker.GetPower() == 0) continue;

            // * 510.1b
            var cs = attacker.CombatState!;
            if (!cs.IsBlocked)
            {
                var target = cs.AttackTarget.GetDamageAssignmentTarget();
                if (target is not null)
                {
                    result.Add(new(
                        new CombatDamageSource(attacker),
                        target)
                    );
                    
                }
                continue;
            }

            // * 510.1c
            var blockers = cs.BlockedBy;
            if (blockers.Count == 0) continue;
            if (blockers.Count == 1)
            {
                result.Add(new(
                    new CombatDamageSource(attacker),
                    new PermanentDamageTarget(blockers[0])
                ));
            }

            // 2+ blockers
            if (blockers.Count > 1)
            {
                throw new NotImplementedException();
            }

            // * 510.1d
            // ! it is assumed that the attacker is on the battlefield (since it was fetched from GetAttackingPermanents())
            foreach (var blocker in blockers)
            {
                if (!blockMap.ContainsKey(blocker))
                    blockMap[blocker] = [];
                blockMap[blocker].Add(attacker);
            }

            foreach (var (blocker, blocked) in blockMap)
            {
                if (blocked.Count == 1)
                {
                    result.Add(new(
                        new CombatDamageSource(blocker),
                        new PermanentDamageTarget(blocked[0])
                    ));
                    continue;
                }

                // 2+ attackers blocked
                throw new NotImplementedException();
            }
        }

        return [.. result];
    }
}
using Mattock.Core.Matches.Combat.AttackDeclarations;
using Mattock.Core.Matches.Combat.AttackDeclarations.Targets;
using Mattock.Core.Matches.Permanents.Statuses;
using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;

namespace Mattock.Core.Matches.Permanents;

public class Permanent
{
    public string Pid { get; }
    public Match Match { get; }
    public Card Card { get; }
    public Player? Controller { get; private set; }

    public PermanentStatus Tapped { get; }
    public PermanentStatus Flipped { get; }
    public PermanentStatus FaceUp { get; }
    public PermanentStatus PhasedIn { get; }

    public Dictionary<PermanentStatusType, PermanentStatus> StatusMap { get; }

    public bool HasSummoningSickness { get; set; }

    public IAttackDeclarationTarget? AttackTarget { get; private set; }

    public Permanent(Card card)
    {
        Pid = card.Match.Battlefield.GeneratePid();
        Match = card.Match;
        Card = card;
        Controller = null;
        AttackTarget = null;

        Tapped = new(PermanentStatusType.Tapped, false);
        Flipped = new(PermanentStatusType.Flipped, false);
        FaceUp = new(PermanentStatusType.FaceUp, true);
        PhasedIn = new(PermanentStatusType.PhasedIn, true);

        StatusMap = new()
        {
            { Tapped.Type, Tapped },
            { Flipped.Type, Flipped },
            { FaceUp.Type, FaceUp },
            { PhasedIn.Type, PhasedIn },
        };

        HasSummoningSickness = true;
    }

    public bool IsAttacking() => AttackTarget is not null;

    public PermanentStatus GetStatus(PermanentStatusType type) => StatusMap[type];

    public string GetDisplayName() => $"[{Pid}]";

    public Player GetOwner() => Match.Players[Card.OwnerIdx];

    public void SetController(Player controller)
    {
        Controller = controller;
    }

    public bool HasType(string type)
    {
        // TODO
        return Card.HasType(type);
    }

    public bool IsControlledBy(int playerIdx)
    {
        // TODO?
        return Controller!.Idx == playerIdx;
    }

    public bool IsUntapped() => !Tapped.Value;

    public bool IsTapped() => Tapped.Value;

    public bool HasName(string name)
    {
        // TODO 
        return Card.HasName(name);
    }

    public AttackDeclaration[] GetAvailableAttackDeclarations()
    {
        if (!HasType(CardTypes.Creature))
            return [];
        if (IsTapped())
        {
            // TODO some effects change this
            return [];
        }

        // TODO haste
        if (HasSummoningSickness && Match.Config.SummoningSickness) return [];

        List<AttackDeclaration> result = [];

        // players
        foreach (var player in Match.Players)
        {
            if (player == Controller) continue;

            // TODO checks for whether can attack or not
            result.Add(new()
            {
                Attacker = this,
                Target = new PlayerAttackDeclarationTarget()
                {
                    Target = player
                }
            });
        }

        // TODO planeswalkers
        // TODO battles

        return [.. result];
    }

    public void SetAttackTarget(IAttackDeclarationTarget target)
    {
        // TODO
        AttackTarget = target;
    }

    public void RemoveAttackTarget()
    {
        AttackTarget = null;
    }

    public Task RemoveFromCombat()
    {
        RemoveAttackTarget();
        return Task.CompletedTask;
    }
}
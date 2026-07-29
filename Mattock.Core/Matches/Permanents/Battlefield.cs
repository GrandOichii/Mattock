using Mattock.Core.Matches.Players;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Zones;

namespace Mattock.Core.Matches.Permanents;

public class Battlefield(Match match) : ICardZone
{
    private readonly List<Permanent> _permanents = [];

    private int _lastPid = 0;

    public string GeneratePid() => $"p{++_lastPid}";

    public string GetZoneName() => "Battlefield";

    public void Remove(Card card)
    {
        // TODO
        throw new NotImplementedException();
    }

    public Permanent? GetPermanentById(string id) => _permanents.SingleOrDefault(p => p.Card.Id == id);
    public Permanent? GetPermanentByPid(string pid) => _permanents.SingleOrDefault(p => p.Pid == pid);

    public async Task<string?> MoveCard(Card card, Player controller)
    {
        return match.MoveCard(
            card,
            CardZoneChangeType.Bottom,
            new CardZoneChanger(
                this,
                _permanents,
                controller
            )
        );
    }

    public Permanent[] GetPermanents()
    {
        // TODO check if any need to be put in graveyard
        return [ .. _permanents ];
    }

    public Permanent[] GetPermanentsControlledBy(Player player)
    {
        return [.. _permanents.Where(p => p.GetController() == player)];
    }

    public Permanent[] GetInCombatPermanents()
    {
        // TODO sus
        return [ .. _permanents.Where(p => p.IsAttacking()) ];
    }

    public Permanent[] GetAttackingPermanents()
    {
        return [ .. _permanents.Where(p => p.IsAttacking()) ];
    }

    public Permanent[] GetAttackingPermanents(Player player)
    {
        return [ .. _permanents.Where(p => 
            p.CombatState is not null && 
            p.CombatState.AttackTarget.BelongsTo(player))
        ];
    }

    // public ICardZoneChanger GetCardZoneChanger(Player controller)
    //     => new CardZoneChanger(this, _permanents, controller);

    class CardZoneChanger(
        Battlefield battlefield,
        List<Permanent> permanents,
        Player controller
    ) : ICardZoneChanger
    {
        
        public bool Accepts(Card card)
        {
            return !card.IsSorcery() && !card.IsInstant();
        }

        public string Do(Card card, CardZoneChangeType type)
        {
            // * type doesn't matter
            var permanent = new Permanent(card, controller);
            permanents.Add(permanent);
            return permanent.Pid;
        }

        public ICardZone GetTargetZone()
            => battlefield;
    }
}
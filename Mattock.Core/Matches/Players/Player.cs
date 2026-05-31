using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Cards.CardZones;
using Mattock.Core.Matches.Players.Controllers;
using Mattock.Core.Setup;

namespace Mattock.Core.Matches.Players;

public class Player
{
    // properties

    public Match Match { get; }
    public int Idx { get; }
    public PlayerSetup Setup { get; }
    public IPlayerController Controller { get; }
    public Life Life { get; }
    public ManaPool ManaPool { get; }

    public Deck Deck { get; }
    public Hand Hand { get; }

    private bool _deckFormed;

    // constructors

    public Player(
        Match match,
        int idx,
        PlayerSetup setup
    )
    {
        Match = match;
        Idx = idx;
        Setup = setup;
        Controller = setup.Controller;

        Life = new(this);
        ManaPool = new(this);
        Deck = new(this);
        Hand = new(this);

        _deckFormed = false;
    }

    // methods

    public bool IsActive() => Idx == Match.ActivePlayerIdx;


    public bool IsNonActive() => Idx != Match.ActivePlayerIdx;


    public string GetDisplayName() => $"{Setup.Name} [{Idx}]";


    public void FormDeck()
    {
        if (_deckFormed)
            throw new Exception($"Called {nameof(FormDeck)} on player {GetDisplayName()}, whose deck is already formed");
        _deckFormed = true;

        foreach (var insert in Setup.Deck.MainDeck)
        {
            for (int i = 0; i < insert.Amount; ++i)
            {
                var card = new Card(this, insert.Card);
                Deck.AddRaw(card);
            }
        }

        Deck.Shuffle();
    }

    public void Draw(int amount)
    {
        for (; amount > 0; --amount)
        {
            DrawSingle();
        }
    }

    public void DrawSingle()
    {
        var card = Deck.GetLast();
        if (card is null)
        {
            if (!Match.Config.GameLossIfRequiredToDrawFromEmptyDeck)
                return;

            throw new NotImplementedException();
        }

        Match.MoveCard(
            card,
            Deck,
            Hand,
            CardZoneChangeType.Bottom
        );
    }
}
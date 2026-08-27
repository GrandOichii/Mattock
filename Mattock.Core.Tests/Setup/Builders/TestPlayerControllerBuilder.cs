using Mattock.Core.Tests.Setup.Builders.ChoiceBuilders;

namespace Mattock.Core.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    private string _name;
    private DeckTemplate _deck;
    private int _teamIdx;

    public CommandChoicesBuilder CommandChoices { get; }
    public PlayersChoicesBuilder PlayersChoices { get; }
    public PermanentsChoicesBuilder PermanentsChoices { get; }
    public StringChoicesBuilder StringChoices { get; }
    public CardChoicesBuilder CardChoices { get; }
    public CostCollectionChoicesBuilder CostCollectionChoices { get; }
    public ManaPaymentChoicesBuilder ManaPaymentChoices { get; }
    public AttackDeclarationsChoicesBuilder AttackDeclarationsChoices { get; }
    public BlockDeclarationsChoicesBuilder BlockDeclarationsChoices { get; }

    public TestPlayerControllerBuilder(string name, int teamIdx)
    {
        _name = name;
        _teamIdx = teamIdx;
        _deck = new()
        {
            MainDeck = []
        };

        CommandChoices = new(this);
        PlayersChoices = new(this);
        PermanentsChoices = new(this);
        StringChoices = new(this);
        CardChoices = new(this);
        CostCollectionChoices = new(this);
        ManaPaymentChoices = new(this);
        AttackDeclarationsChoices = new(this);
        BlockDeclarationsChoices = new(this);
    }

    public PlayersChoicesBuilder ChoosePlayers => PlayersChoices;
    public PermanentsChoicesBuilder ChoosePermanents => PermanentsChoices;
    public StringChoicesBuilder ChooseString => StringChoices;
    public CardChoicesBuilder ChooseCard => CardChoices;
    public CostCollectionChoicesBuilder ChooseCostCollection => CostCollectionChoices;
    public ManaPaymentChoicesBuilder PayMana => ManaPaymentChoices;
    public CommandChoicesBuilder Act => CommandChoices;
    public AttackDeclarationsChoicesBuilder DeclareAttack => AttackDeclarationsChoices; 
    public BlockDeclarationsChoicesBuilder DeclareBlock => BlockDeclarationsChoices; 

    public TestPlayerControllerBuilder SetDeck(DeckTemplate deck)
    {
        _deck = deck;
        return this;
    }

    public TestPlayerController Build(TestSessionWrapper match)
    {
        return new(
            match,
            _name,
            _deck,
            _teamIdx,
            CommandChoices.Queue,
            PlayersChoices.Queue,
            PermanentsChoices.Queue,
            StringChoices.Queue,
            CardChoices.Queue,
            CostCollectionChoices.Queue,
            ManaPaymentChoices.Queue,
            AttackDeclarationsChoices.Queue,
            BlockDeclarationsChoices.Queue
        );
    }
}

using Mattock.Core.Matches.Permanents;
using Mattock.Core.Matches.Players.Actions;
using Mattock.Core.Matches.Players.Cards;
using Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;
using Mattock.Core.Matches.Players.Costs;

namespace Mattock.Core.Matches.Players.Controllers;

// public class PlayerResponsesRecord
// {
//     public List<string>
// }

public class RecorderPlayerControllerWrapper(IPlayerController controller)
    : PlayerControllerWrapper(controller)
{

    public override Task HandleCardChoice(Card? choice, Player player, Card[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public override Task HandleCommandChoice(ICommand choice, Player player, ICommand[] choices)
    {
        throw new NotImplementedException();
    }

    public override Task HandleCostCollectionChoice(CostCollection? choice, Player player, CostCollection[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public override Task HandleManaPaymentChoice(IManaPaymentChoice choice, Player player, IManaPaymentChoice[] options, string hint)
    {
        throw new NotImplementedException();
    }

    public override Task HandlePermanentsChoice(Permanent[] choices, Player player, Permanent[] options, int min, int max, string hint)
    {
        throw new NotImplementedException();
    }

    public override Task HandlePlayersChoice(Player[] choices, Player player, Player[] options, int min, int max, string hint)
    {
        throw new NotImplementedException();
    }

    public override Task HandleStringChoice(string? choice, Player player, string[] options, string hint)
    {
        throw new NotImplementedException();
    }
}
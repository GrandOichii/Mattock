namespace Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

public interface IManaPaymentChoice
{
    
    string ToDisplayString();

    Task Process(ManaPayment payment);
}
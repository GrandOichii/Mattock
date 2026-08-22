using Mattock.Core.Matches.Rollback;

namespace Mattock.Core.Matches.Players.Controllers.ManaPaymentChoices;

public interface IManaPaymentChoice
{
    
    string ToDisplayString();

    Task<RollbackRequest?> Process(ManaPayment payment);
}
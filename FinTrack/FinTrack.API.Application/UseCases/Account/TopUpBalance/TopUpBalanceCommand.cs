using MediatR;
namespace FinTrack.API.Application.UseCases.Account.TopUpBalance
{
    /// <summary>
    /// Represents MediatR command to top up balance.
    /// Returns balance of the account
    /// </summary>
    /// <param name="accountId">Destination account id</param>
    /// <param name="amount">
    /// Deposit amount.
    /// Must be positive
    /// </param>
    public record TopUpBalanceCommand(Guid accountId, decimal amount) : IRequest<decimal>;
    
}

using MediatR;

namespace FinTrack.API.Application.UseCases.Account.DebitBalance
{
    /// <summary>
    /// Represents MediatR command to debit balance.
    /// Returns balance of the account
    /// </summary>
    /// <param name="accountId">Source account id</param>
    /// <param name="amount">
    /// Debit amount.
    /// Must be positive
    /// </param>
    public record DebitBalanceCommand(Guid accountId, decimal amount) : IRequest<decimal>;
    
}

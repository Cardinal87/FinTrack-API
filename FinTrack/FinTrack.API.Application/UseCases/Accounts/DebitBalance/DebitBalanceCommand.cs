using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DebitBalance
{
    /// <summary>
    /// Represents MediatR command to debit balance.
    /// Returns <see cref="ValueResult{T}"/> with balance of the account
    /// </summary>
    /// <param name="accountId">Source account id</param>
    /// <param name="amount">
    /// Debit amount.
    /// Must be positive
    /// </param>
    public record DebitBalanceCommand(Guid accountId, decimal amount) : IRequest<ValueResult<decimal>>;
    
}



using FinTrack.API.Application.Common;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.CreateTransaction
{
    /// <summary>
    /// Represents MediatR command for creating transaction.
    /// Returns <see cref="ValueResult{T}"/> with <see cref="Guid"/> of created transaction
    /// </summary>
    /// <param name="userId">
    /// Id of the user that creates the transaction
    /// </param>
    /// <param name="amount">
    /// Amount of the transaction.
    /// Must be positive
    /// </param>
    /// <param name="fromAccountId">
    /// Source account id.
    /// Must not match with <paramref name="toAccountId"/> 
    /// </param>
    /// <param name="toAccountId">
    /// Destination account id.
    /// Must not match with <paramref name="fromAccountId"/>
    /// </param>
    public record CreateTransactionCommand(Guid userId, decimal amount, Guid fromAccountId, Guid toAccountId) : IRequest<ValueResult<Guid>>;
    
}

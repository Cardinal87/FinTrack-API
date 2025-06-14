

using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.CreateTransaction
{
    /// <summary>
    /// Represents MediatR command for creating transaction.
    /// Returns Guid of created transaction
    /// </summary>
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
    public record CreateTransactionCommand(decimal amount, Guid fromAccountId, Guid toAccountId) : IRequest<Guid>;
    
}

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;


namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetTransactionById
{
    /// <summary>
    /// Represents MediatR command for getting transaction by id.
    /// Returns <see cref="ValueResult{T}"/> with <see cref="Transaction"/> if it exists and
    /// user has permission to get it
    /// else returns <see langword="null"/>.
    /// </summary>
    /// <param name="transactionId">Transaction id</param>
    /// <param name="userId">Id of the user that invokes command</param>
    /// <param name="roles">User roles</param>
    public record GetTransactionByIdQuery(Guid userId, IReadOnlyCollection<string> roles, Guid transactionId) : IRequest<ValueResult<Transaction>>;
}

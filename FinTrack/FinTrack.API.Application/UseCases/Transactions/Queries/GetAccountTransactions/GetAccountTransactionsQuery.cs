using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetAccountTransactions
{
    /// <summary>
    /// Represents MediatR command for getting all account transactions.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="Transaction"/>s
    /// </summary>
    /// <param name="accountId">Account id</param>
    /// <param name="userId">Id of the user that invokes command</param>
    /// <param name="roles">User roles</param>
    /// <param name="pageNumber">page number of the paginated result</param>
    /// <param name="pageSize">page size of the paginated result</param>
    public record GetAccountTransactionsQuery(Guid userId,
                                                IReadOnlyCollection<string> roles,
                                                Guid accountId,
                                                int pageNumber,
                                                int pageSize) : IRequest<ValueResult<IReadOnlyCollection<Transaction>>>;
}

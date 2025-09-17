using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.Queries.GetTransactionsByDate
{

    /// <summary>
    /// Represents MediatR command for getting all transactions by specified date.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="Transaction"/>s
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="roles">User roles</param>
    /// <param name="date">Date of transaction</param>
    /// <param name="pageNumber">page number of the paginated result</param>
    /// <param name="pageSize">page size of the paginated result</param>
    public record GetTransactionsByDateQuery(Guid userId,
                                               IReadOnlyCollection<string> roles,
                                               DateOnly date,
                                               int pageNumber,
                                               int pageSize) : IRequest<ValueResult<IReadOnlyCollection<Transaction>>>;
   
}

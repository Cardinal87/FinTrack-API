using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.Queries.GetAllAccounts
{
    /// <summary>
    /// Represents MediatR query for getting all accounts.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="Account"/>s
    /// </summary>
    /// <param name="pageNumber">page number of the paginated result</param>
    /// <param name="pageSize">page size of the paginated result</param>
    public record GetAllAccountsQuery(int pageNumber, int pageSize) : IRequest<ValueResult<IReadOnlyCollection<Account>>>;
}

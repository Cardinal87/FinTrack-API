

using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Users.Queries.GetAllUsers
{
    /// <summary>
    /// Represents MediatR command for getting all users.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="User"/>s
    /// </summary>
    /// <param name="pageNumber">page number of the paginated result</param>
    /// <param name="pageSize">page size of the paginated result</param>
    public record GetAllUsersQuery(int pageNumber, int pageSize): IRequest<ValueResult<IReadOnlyCollection<User>>>;
}


using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.GetTransactionsByTimeInterval
{

    /// <summary>
    /// Represents MediatR command to get transactions by time interval.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="Transaction"/>s
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="roles">User roles</param>
    /// <param name="from">start of the interval</param>
    /// <param name="to">end of the interval</param>
    public record GetTransactionsByTimeIntervalCommand(Guid userId,
                                                       IEnumerable<string> roles,
                                                       DateTime from,
                                                       DateTime to) : IRequest<ValueResult<IReadOnlyCollection<Transaction>>>;
    
}


using FinTrack.API.Application.Common;
using FinTrack.API.Core.Entities;
using MediatR;

namespace FinTrack.API.Application.UseCases.Transactions.GetAccountTransactions
{
    /// <summary>
    /// Represents MediatR command for getting all account transactions.
    /// Returns <see cref="ValueResult{T}"/> constains <see cref="IReadOnlyCollection{T}"/> 
    /// with <see cref="Transaction"/>s
    /// </summary>
    /// <param name="accountId">Account id</param>
    /// <param name="userId">Id of the user that invokes command</param>
    /// <param name="roles">User roles</param>
    public record GetAccountTransactionsCommand(Guid userId,
                                                IReadOnlyCollection<string> roles,
                                                Guid accountId) : IRequest<ValueResult<IReadOnlyCollection<Transaction>>>;
}

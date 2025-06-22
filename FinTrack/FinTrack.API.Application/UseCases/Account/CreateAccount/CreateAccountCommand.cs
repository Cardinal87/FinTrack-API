
using MediatR;

namespace FinTrack.API.Application.UseCases.Account.CreateAccount
{
    /// <summary>
    /// Represents MediatR command to create account.
    /// Returns id of the created account
    /// </summary>
    /// <param name="userId">
    /// the ID of the user for whom the account will be created
    /// </param>
    public record CreateAccountCommand(Guid userId) : IRequest<Guid>;
}

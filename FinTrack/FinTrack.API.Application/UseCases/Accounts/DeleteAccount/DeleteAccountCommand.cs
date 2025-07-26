

using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteAccount
{
    /// <summary>
    /// Represents MediatR command for deleting account.
    /// </summary>
    /// <param name="accountId">
    /// Id of the existing account
    /// </param>
    public record DeleteAccountCommand(Guid accountId) : IRequest;
    
}

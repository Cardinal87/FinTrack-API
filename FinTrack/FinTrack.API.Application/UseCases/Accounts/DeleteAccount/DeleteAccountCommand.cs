

using MediatR;

namespace FinTrack.API.Application.UseCases.Accounts.DeleteAccount
{
    /// <summary>
    ///  Represents MediatR command to delete account.
    /// </summary>
    /// <param name="userId">
    /// Id of the existing user
    /// </param>
    public record DeleteAccountCommand(Guid userId) : IRequest;
    
}

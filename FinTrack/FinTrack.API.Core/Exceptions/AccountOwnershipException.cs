
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{

    /// <summary>
    /// Exception for attempt to assign an incorrect account to user
    /// </summary>
    /// <remarks>
    /// Cases
    /// <list type="bullet">
    /// <item>Account.UserId != User.Id</item>
    /// </list>
    /// </remarks>

    public class AccountOwnershipException : DomainException
    {
        /// <summary>
        /// Id of the destination user
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Id by which the account refers to the user
        /// </summary>
        public Guid AccountRefId { get; }
        public AccountOwnershipException(Guid userId,
                                         Guid accountRefId) : base("Account does not belong to this user")
        {
            UserId = userId;
            AccountRefId = accountRefId;
        }

    }
}

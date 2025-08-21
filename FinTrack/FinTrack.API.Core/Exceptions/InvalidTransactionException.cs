using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for transactions
    /// </summary>
    /// <remarks>
    /// Must be thrown in case of incorrect transaction creating
    /// with indicating the reason
    /// <para>
    /// Cases:
    /// <list type="bullet">
    /// <item>date of transaction is greater than <see cref="DateTime.UtcNow"/></item>
    /// <item>Trasaction.FromAccountId == Transaction.ToAccountId</item>
    /// </list>
    /// </para>
    /// </remarks>
    public sealed class InvalidTransactionException : DomainException
    {
        /// <summary>
        /// Short description of the reason of the exception
        /// </summary>
        public string Reason => (string)Data["Reason"]!;
        
        /// <summary>
        /// </summary>
        /// <param name="reason">reason of the exception</param>
        public InvalidTransactionException(string reason) 
            : base($"Transaction invalid: {reason}")
        {
            Data.Add("Reason", reason);
        }
    }
}

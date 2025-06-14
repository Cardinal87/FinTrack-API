using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for transactions
    /// </summary>
    /// <remarks>
    /// Must be thrown in case of incorrect transaction handling
    /// with indicating the reason
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

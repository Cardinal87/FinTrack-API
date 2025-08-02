
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for incorrect operation amount
    /// </summary>
    /// <remarks>
    /// Must be thrown when amount <= 0
    /// </remarks>
    public class IncorrectAmountException : DomainException
    {
        /// <summary>
        /// Invalid amount
        /// </summary>
        public decimal Amount { get; }
        
        /// <summary>
        /// </summary>
        /// <param name="amount">invalid amount</param>
        /// <param name="message">exception message</param>
        public IncorrectAmountException(decimal amount, string message) : base(message)
        {
            Amount = amount;
        }
    }
}

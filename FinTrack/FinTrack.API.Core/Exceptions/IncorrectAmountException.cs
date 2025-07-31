
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    public class IncorrectAmountException : DomainException
    {
        public decimal Amount { get; }
        
        public IncorrectAmountException(decimal amount, string message) : base(message)
        {
            Amount = amount;
        }
    }
}

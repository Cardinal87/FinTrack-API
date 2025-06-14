

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for debiting funds
    /// </summary>
    /// <remarks>
    /// Must be thrown when debit amount is greater than current balance
    /// </remarks>
    public sealed class InsufficientFundsException : Exception
    {
        /// <summary>
        /// Current balance of the account
        /// </summary>
        public decimal CurrentBalance => (decimal)Data["CurrentBalance"]!;

        /// <summary>
        /// Required debit amount
        /// </summary>
        public decimal RequiredAmount => (decimal)Data["Required"]!;
        
        /// <summary>
        /// </summary>
        /// <param name="balance">balance of the account</param>
        /// <param name="required">required debit amount</param>
        public InsufficientFundsException(decimal balance, decimal required) :
            base($"InsufficientFundsException. Current balance: {balance}, required: {required}")
        {
            Data.Add("CurrentBalance", balance);
            Data.Add("Required" , required);
        }
         
    }
}

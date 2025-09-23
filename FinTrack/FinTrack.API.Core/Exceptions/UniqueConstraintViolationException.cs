
namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for violation unique contstaint when saving data
    /// </summary>
    public class UniqueConstraintViolationException : Exception
    {
        /// <summary>
        /// Property whose value violates unique constraint
        /// </summary>
        public string Property { get; private set; }
        

        public UniqueConstraintViolationException(string property) : base("Dublicate value violation")
        { 
            Property = property;
        }
    }
}


namespace FinTrack.API.Core.Common
{
    
    /// <summary>
    /// Represents base class for domain layer exceptions
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}

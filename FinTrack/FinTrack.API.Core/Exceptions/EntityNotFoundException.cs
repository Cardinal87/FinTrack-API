
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    /// <summary>
    /// Exception for showing that entity cannot be find
    /// </summary>
    /// <remarks>
    /// Must be thrown when entity cannot be find by
    /// id or by amother key
    /// </remarks>
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}

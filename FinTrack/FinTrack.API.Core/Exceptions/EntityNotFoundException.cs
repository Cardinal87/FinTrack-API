
using FinTrack.API.Core.Common;

namespace FinTrack.API.Core.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }
    }
}

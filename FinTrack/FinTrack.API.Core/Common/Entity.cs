
namespace FinTrack.API.Core.Common
{
    /// <summary>
    /// presents abstract class of Entity
    /// </summary>
    
    public abstract class Entity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public override bool Equals(object? obj)
        {
            if (obj is Entity entity)
            {
                return entity.Id == Id;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

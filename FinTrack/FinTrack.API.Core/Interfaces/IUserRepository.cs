using FinTrack.API.Core.Entities;

namespace FinTrack.API.Core.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Begins tracking the given entity as "Added"
        /// </summary>
        /// <param name="user">the entity to track </param>
        void Add(User user);

        /// <summary>
        /// Begins tracking the given entity as "Updated"
        /// </summary>
        /// <param name="user">the entity to update</param>
        /// <exception cref="KeyNotFoundException">the given entity does not exist</exception>
        /// <returns><see cref="Task"/></returns>
        Task UpdateAsync(User user);

        /// <summary>
        /// Begins tracking the given entity as "Deleted"
        /// </summary>
        /// <param name="id">id of the entity to delete</param>
        /// <exception cref="KeyNotFoundException">the given entity does not exist</exception>
        /// <returns><see cref="Task"/></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Returns all entities asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="IEnumerable{T}"/> with <see cref="User"/> entities
        /// </returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Returns entity with given id asynchronously
        /// </summary>
        /// <param name="id">id of entity</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="User"/> with given <paramref name="id"/> or <see langword="null"/> if entity does not exist 
        /// </returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns entity with given email asynchronously
        /// </summary>
        /// <param name="email">email of the entity</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="User"/> with given <paramref name="id"/> or <see langword="null"/> if 
        ///     entity with given email does not exist 
        /// </returns>
        Task<User?> GetByEmailAsync(string email);


        /// <summary>
        /// Asynchronously save all changes maked in repository
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SaveChangesAsync();
    }
}

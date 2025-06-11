using FinTrack.API.Core.Entities;

namespace FinTrack.API.Core.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Begins tracking the given entity as "Added"
        /// </summary>
        /// <param name="account">the entity to track </param>
        void Save(Account account);

        /// <summary>
        /// Begins tracking the given entity as "Updated"
        /// </summary>
        /// <param name="account">the entity to update</param>
        /// <exception cref="KeyNotFoundException">the given entity does not exist</exception>
        /// <returns><see cref="Task"/></returns>
        Task UpdateAsync(Account account);

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
        ///     The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Account>> GetAllAsync();

        /// <summary>
        /// Returns entity with given id asynchronously
        /// </summary>
        /// <param name="id">id of entity</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="Account"/> with given <paramref name="id"/> or <see langword="null"/> if entity does not exist 
        /// </returns>
        Task<Account?> GetByIdAsync(Guid id);

        /// <summary>
        /// Asynchronously save all changes maked in repository
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SaveChangesAsync();
    }
}

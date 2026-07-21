using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Exceptions;
namespace FinTrack.API.Core.Interfaces
{
    public interface IAccountRepository
    {
        /// <summary>
        /// Asynchronously add provided entity
        /// </summary>
        /// <param name="account">the entity to track </param>
        Task AddAsync(Account account);

        /// <summary>
        /// Asynchronously update provided entity
        /// </summary>
        /// <param name="account">the entity to update</param>
        /// <exception cref="EntityNotFoundException">the given entity does not exist</exception>
        /// <returns><see cref="Task"/></returns>
        Task UpdateAsync(Account account);

        /// <summary>
        /// Asynchronously delete entity by id
        /// </summary>
        /// <param name="id">id of the entity to delete</param>
        /// <exception cref="EntityNotFoundException">the given entity does not exist</exception>
        /// <returns><see cref="Task"/></returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Returns all entities asynchronously
        /// </summary>
        /// <param name="pageNumber">page number of the paginated result (default: 1)</param>
        /// <param name="pageSize">page size of the paginated result, maximum size is 1000 (default: 50)</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="IEnumerable{T}"/> size of <paramref name="pageSize"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Account>> GetAllAsync(int pageNumber = 1, int pageSize = 50);

        /// <summary>
        /// Returns ids of all user accounts
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>
        /// <see cref="IEnumerable{T}"/> contains <see cref="Guid"/>
        /// </returns>
        Task<IEnumerable<Guid>> GetAccountIdsByUserIdAsync(Guid id);


        /// <summary>
        /// Returns entity with given id asynchronously
        /// </summary>
        /// <param name="id">id of entity</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="Account"/> with given <paramref name="id"/> or <see langword="null"/> if entity does not exist 
        /// </returns>
        Task<Account?> GetByIdAsync(Guid id);

    }
}

using FinTrack.API.Core.Entities;

namespace FinTrack.API.Core.Interfaces
{
    public interface ITransactionRepository
    {
        /// <summary>
        /// Begins tracking the given entity as "Added"
        /// </summary>
        /// <param name="transaction">the entity to track </param>
        void Add(Transaction transaction);


        /// <summary>
        /// Returns all entities asynchronously
        /// </summary>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetAllAsync();

        /// <summary>
        /// Returns entity with given id asynchronously
        /// </summary>
        /// <param name="id">id of entity</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="Transaction"/> with given <paramref name="id"/> or <see langword="null"/> if entity does not exist 
        /// </returns>
        Task<Transaction?> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns the <see cref="Transaction"/> with given date.
        /// Compares only <see cref="DateTime.Date"/>
        /// </summary>
        /// <param name="date">date of the transaction</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///      The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date);


        /// <summary>
        /// Returns all entities between <paramref name="fromDate"/> and <paramref name="toDate"/> inclusive
        /// </summary>
        /// <param name="fromDate">start of the range</param>
        /// <param name="toDate">end of the range</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Asynchronously save all changes maked in repository
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SaveChangesAsync();
    }
}

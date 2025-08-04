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
        /// Returns the <see cref="Transaction"/> with given <see cref="DateOnly"/>
        /// </summary>
        /// <param name="date">date of the transaction</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///      The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date);

        /// <summary>
        /// Returns the <see cref="Transaction"/>s with given <paramref name="date"/>
        /// and if <paramref name="accountIds"/> contains <see cref="Transaction.FromAccountId"/> or
        /// <see cref="Transaction.ToAccountId"/>
        /// </summary>
        /// <param name="date">date of the transaction</param>
        /// <param name="accountIds">id of accounts involved in the transaction</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///      The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetByDateAsync(DateOnly date, IEnumerable<Guid> accountIds);


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
        /// Returns all <see cref="Transaction"/>s between <paramref name="fromDate"/> and <paramref name="toDate"/> inclusive 
        /// and the <see cref="Transaction.FromAccountId"/> or <see cref="Transaction.ToAccountId"/> of which is contained in <paramref name="accountIds"/>
        /// </summary>
        /// <param name="fromDate">start of the range</param>
        /// <param name="toDate">end of the range</param>
        /// <param name="accountIds">id of accounts involved in the transaction</param>
        /// <returns>
        ///     <see cref="Task"/>.
        ///     The task result contains a <see cref="IEnumerable{T}"/> with <see cref="Transaction"/> entities
        /// </returns>
        Task<IEnumerable<Transaction>> GetFromToDateAsync(DateTime fromDate, DateTime toDate, IEnumerable<Guid> accountIds);

        /// <summary>
        /// Asynchronously save all changes maked in repository
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task SaveChangesAsync();
    }
}

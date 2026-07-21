
namespace FinTrack.API.Core.Interfaces
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Atomically save all changes made in current context
        /// </summary>
        /// <param name="ct">Cancallation token</param>
        /// <returns></returns>
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

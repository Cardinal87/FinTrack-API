using FinTrack.API.Core.Interfaces;

namespace FinTrack.API.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseClient _databaseClient;

        public UnitOfWork(DatabaseClient databaseClient)
        {
            _databaseClient = databaseClient;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _databaseClient.SaveChangesAsync(ct);
        }
    }
}

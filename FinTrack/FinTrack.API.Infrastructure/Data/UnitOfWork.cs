using FinTrack.API.Core.Exceptions;
using FinTrack.API.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
            try
            {
                await _databaseClient.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                var property = pgEx.ConstraintName ?? "Unknown";
                throw new UniqueConstraintViolationException(property);
            }
        }
    }
}


using FinTrack.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices.Marshalling;
using Testcontainers.PostgreSql;

namespace FinTrack.IntegrationTests.Common
{
    public class DatabaseTestBase : IAsyncLifetime
    {
        protected DatabaseClient _client = null!;
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithDatabase("test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();
        async public Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        virtual async public Task InitializeAsync()
        {
            await _container.StartAsync();

            var options = new DbContextOptionsBuilder<DatabaseClient>()
                .UseNpgsql(_container.GetConnectionString())
                .Options;

            _client = new DatabaseClient(options);
            await _client.Database.MigrateAsync();

        }
    }
}

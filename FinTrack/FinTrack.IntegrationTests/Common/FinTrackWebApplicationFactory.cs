
using FinTrack.API.Core.Interfaces;
using FinTrack.API.TestMocks.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FinTrack.IntegrationTests.Common
{
    public class FinTrackWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //Remove all services to work with database
                services.RemoveAll<DbContext>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<IUserRepository>();
                services.RemoveAll<IAccountRepository>();
                services.RemoveAll<ITransactionRepository>();

                //Add mocks to imitate database
                services.AddSingleton<IUserRepository, UserRepositoryMock>();
                services.AddSingleton<IAccountRepository, AccountRepositoryMock>();
                services.AddSingleton<ITransactionRepository, TransactionRepositoryMock>();
            });
        }
    }
}

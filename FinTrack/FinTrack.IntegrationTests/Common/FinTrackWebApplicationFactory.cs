
using FinTrack.API.Core.Common;
using FinTrack.API.Core.Entities;
using FinTrack.API.Core.Interfaces;
using FinTrack.API.Application.Interfaces;
using FinTrack.API.Infrastructure.Caching.Decorators;
using FinTrack.API.Infrastructure.Identity.Services;
using FinTrack.API.Infrastructure.Interfaces;
using FinTrack.API.Middleware;
using FinTrack.API.TestMocks.Builders;
using FinTrack.API.TestMocks.Cache;
using FinTrack.API.TestMocks.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;
using Moq;

namespace FinTrack.IntegrationTests.Common
{
    public class FinTrackWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        public UserRepositoryMock UserRepositoryMock { get;private set; } = new();
        public AccountRepositoryMock AccountRepositoryMock { get; private set; } = new();
        public TransactionRepositoryMock TransactionRepositoryMock { get; private set; } = new();
        public RefreshTokenRepositoryMock RefreshTokenRepositoryMock { get; private set; } = new();
        public CacheServiceMock CacheServiceMock { get; private set; } = new();
        public Mock<IUnitOfWork> UnitOfWorkMock { get; private set; } = new(); 
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                //Remove all services to work with database
                services.RemoveAll<DbContext>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<IUserRepository>();
                services.RemoveAll<IAccountRepository>();
                services.RemoveAll<ITransactionRepository>();
                services.RemoveAll<IRefreshTokenRepository>();

                //Remove all redis specified services
                services.RemoveAll<IConnectionMultiplexer>();
                services.RemoveAll<ICacheService>();

                //Change vault to local signing service
                services.RemoveAll<IJwtSigningService>();
                services.RemoveAll<VaultTokenHeaderHandler>();
                services.AddSingleton<IJwtSigningService, LocalJwtSigningService>();

                //Add mocks to imitate database 
                services.AddSingleton<IUserRepository>(UserRepositoryMock);
                services.AddSingleton<IAccountRepository>(AccountRepositoryMock);
                services.AddSingleton<ITransactionRepository>(TransactionRepositoryMock);
                services.AddSingleton<IRefreshTokenRepository>(RefreshTokenRepositoryMock);
                services.AddSingleton<IUnitOfWork>(UnitOfWorkMock.Object);

                //Add mocks to imitate cache
                services.AddSingleton<ICacheService>(CacheServiceMock);

                //Restore Decorators
                services.Decorate<ICacheService, CacheResilienceDecorator>();
                services.Decorate<IUserRepository, CachedUserRepositoryDecorator>();
                services.Decorate<IAccountRepository, CachedAccountRepositoryDecorator>();
                services.Decorate<ITransactionRepository, CachedTransactionRepositoryDecorator>();

            });

        }
        public void ResetMocks()
        {
            UserRepositoryMock.Reset();
            AccountRepositoryMock.Reset();
            TransactionRepositoryMock.Reset();
            RefreshTokenRepositoryMock.Reset();
            CacheServiceMock.Reset();
        }

        public (User, User) CreateBaseUsers()
        {
            using (var scope = Services.CreateScope())
            {
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();


                var admin = new UserBuilder().WithPassword("pwd", hasher)
                    .WithEmail("admin@email.com")
                    .WithRoles(UserRoles.Admin, UserRoles.User)
                    .Build();

                var user = new UserBuilder().WithPassword("pwd", hasher)
                    .WithEmail("user@email.com")
                    .WithRoles(UserRoles.User)
                    .Build();


                var userAccount = new Account(user.Id);
                var adminAccount = new Account(admin.Id);

                UserRepositoryMock.AddAsync(admin);
                UserRepositoryMock.AddAsync(user);

                return (user, admin);

            }
        }
    }
}

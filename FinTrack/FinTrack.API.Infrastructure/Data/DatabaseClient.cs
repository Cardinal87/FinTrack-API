using FinTrack.API.Infrastructure.Data.Configurations;
using FinTrack.API.Infrastructure.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace FinTrack.API.Infrastructure.Data
{
    public class DatabaseClient : DbContext
    {
        public DbSet<UserDb> Users { get; set; }
        public DbSet<AccountDb> Accounts { get; set; }
        public DbSet<TransactionDb> Transactions { get; set; }
        
        public DatabaseClient(DbContextOptions<DatabaseClient> options) : base(options) { }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        }
    }
}

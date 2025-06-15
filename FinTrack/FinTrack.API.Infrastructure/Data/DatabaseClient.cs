using FinTrack.API.Core.Entities;
using FinTrack.API.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace FinTrack.API.Infrastructure.Data
{
    public class DatabaseClient : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        public DatabaseClient(DbContextOptions<DatabaseClient> options) : base(options) { }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        }
    }
}

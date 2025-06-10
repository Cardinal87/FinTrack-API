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
        
        public DatabaseClient() 
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var options = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Database = "FinTrackDb",
                Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"),
                Username = Environment.GetEnvironmentVariable("POSTGRES_USERNAME")
            };

            optionsBuilder.UseNpgsql(options.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        }
    }
}

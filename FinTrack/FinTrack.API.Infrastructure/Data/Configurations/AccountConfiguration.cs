using FinTrack.API.Infrastructure.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class AccountConfiguration : IEntityTypeConfiguration<AccountDb>
    {
        public void Configure(EntityTypeBuilder<AccountDb> builder)
        {
            builder.ToTable("Accounts");
            
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Balance).HasPrecision(18,2).IsRequired();
            builder.Property(t => t.UserId).IsRequired();


            builder.HasMany(t => t.OutgoingTransactions)
                .WithOne(t => t.FromAccount)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasMany(t => t.IncomingTransactions)
                .WithOne(t => t.ToAccount)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

        }
    }
}

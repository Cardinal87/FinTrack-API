using FinTrack.API.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Balance).HasPrecision(18,2).IsRequired();
            builder.Property(t => t.UserId).IsRequired();
            
            
            builder.HasMany(t => t.OutgoingTransactions)
                .WithOne()
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasMany(t => t.IncomingTransactions)
                .WithOne()
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

        }
    }
}

using FinTrack.API.Infrastructure.Common.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class TransactionConfiguration : IEntityTypeConfiguration<TransactionDTO>
    {
        public void Configure(EntityTypeBuilder<TransactionDTO> builder)
        {
            builder.ToTable("Transactions");
            
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(t => t.FromAccountId).IsRequired();
            builder.Property(t => t.ToAccountId).IsRequired();
            builder.Property(t => t.Date).HasColumnType("timestamp with time zone");

            builder.HasOne<AccountDTO>()
                .WithMany()
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<AccountDTO>()
                .WithMany()
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

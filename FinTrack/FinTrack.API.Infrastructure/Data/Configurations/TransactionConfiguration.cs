using FinTrack.API.Infrastructure.Data.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class TransactionConfiguration : IEntityTypeConfiguration<TransactionDb>
    {
        public void Configure(EntityTypeBuilder<TransactionDb> builder)
        {
            builder.ToTable("Transactions");
            
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
            builder.Property(t => t.FromAccountId).IsRequired();
            builder.Property(t => t.ToAccountId).IsRequired();
            builder.Property(t => t.Date).HasColumnType("timestamp with time zone");
        }
    }
}

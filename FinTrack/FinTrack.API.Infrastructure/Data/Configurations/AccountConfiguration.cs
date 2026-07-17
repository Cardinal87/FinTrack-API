using FinTrack.API.Infrastructure.Common.DTO;
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

            builder.HasOne<UserDb>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}

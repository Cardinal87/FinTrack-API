using Microsoft.EntityFrameworkCore;
using FinTrack.API.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(t => t.Email).IsUnique();
            builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
            builder.Property(t => t.PasswordHash).HasMaxLength(64).IsRequired();
            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();

            builder.HasOne(t => t.Account)
                .WithOne()
                .HasForeignKey<Account>(t => t.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

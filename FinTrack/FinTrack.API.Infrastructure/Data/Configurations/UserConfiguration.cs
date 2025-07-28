using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinTrack.API.Infrastructure.Data.DbEntities;
namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<UserDb>
    {
        public void Configure(EntityTypeBuilder<UserDb> builder)
        {
            builder.ToTable("Users");
            
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(t => t.Email).IsUnique();
            builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
            builder.HasIndex(t => t.Phone).IsUnique();
            builder.Property(t => t.PasswordHash).HasMaxLength(64).IsRequired();
            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
            builder.HasIndex(t => t.Name).IsUnique();
            builder.Property(t => t.Roles).IsRequired();

            builder.HasMany(t => t.Accounts)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FinTrack.API.Infrastructure.Common.DTO;
namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<UserDTO>
    {
        public void Configure(EntityTypeBuilder<UserDTO> builder)
        {
            builder.ToTable("Users");
            
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(t => t.Email).IsUnique();
            builder.Property(t => t.Phone).HasMaxLength(20).IsRequired();
            builder.HasIndex(t => t.Phone).IsUnique();
            builder.Property(t => t.PasswordHash).HasMaxLength(120).IsRequired();
            builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
            builder.HasIndex(t => t.Name).IsUnique();
            builder.Property(t => t.Roles).IsRequired();

        }
    }
}

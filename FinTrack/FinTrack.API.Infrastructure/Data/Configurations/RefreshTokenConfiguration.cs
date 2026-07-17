

using FinTrack.API.Infrastructure.Common.DTO;
using FinTrack.API.Infrastructure.Identity.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.API.Infrastructure.Data.Configurations
{
    class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.Property(x => x.UserId).IsRequired();
            builder.HasIndex(x => x.UserId);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.IsRevoked).IsRequired();
            builder.HasIndex(x => x.ReplacedByTokenId).IsUnique(false);

            builder.HasOne<UserDb>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RefreshToken>()
                .WithOne()
                .HasForeignKey<RefreshToken>(t => t.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.ClientSetNull);


        }
    }
}

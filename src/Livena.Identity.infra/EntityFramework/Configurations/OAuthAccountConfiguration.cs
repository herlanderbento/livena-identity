using Livena.Identity.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

public class OAuthAccountConfiguration : IEntityTypeConfiguration<OAuthAccount>
{
    public void Configure(EntityTypeBuilder<OAuthAccount> builder)
    {
        builder.ToTable("oauth_accounts");

        builder.HasKey(oa => oa.Id);

        builder.Property(oa => oa.UserId).HasColumnName("user_id").IsRequired();

        builder.Property(oa => oa.Provider).HasColumnName("provider").IsRequired().HasMaxLength(50);

        builder
            .Property(oa => oa.ProviderUserId)
            .HasColumnName("provider_user_id")
            .IsRequired()
            .HasMaxLength(255);

        builder
            .Property(oa => oa.AccessToken)
            .HasColumnName("access_token")
            .IsRequired()
            .HasMaxLength(2000);

        builder
            .Property(oa => oa.RefreshToken)
            .HasColumnName("refresh_token")
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(oa => oa.ExpiresAt).HasColumnName("expires_at").IsRequired();

        builder.Property(oa => oa.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(oa => new { oa.UserId, oa.Provider }).IsUnique();

        builder.HasIndex(oa => new { oa.Provider, oa.ProviderUserId }).IsUnique();

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(oa => oa.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

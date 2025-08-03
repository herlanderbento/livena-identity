using Livena.Identity.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

internal class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.ToTable("revoked_tokens");

        builder.HasKey(revokedToken => revokedToken.Id);

        builder.Property(revokedToken => revokedToken.UserId).HasColumnName("user_id").IsRequired();

        builder
            .Property(revokedToken => revokedToken.AccessToken)
            .HasColumnName("access_token")
            .IsRequired();

        builder
            .Property(revokedToken => revokedToken.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired();

        builder
            .Property(revokedToken => revokedToken.RevokedAt)
            .HasColumnName("revoked_at")
            .IsRequired();

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(revokedToken => revokedToken.UserId)
            .HasConstraintName("fk_revoked_tokens_users")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(revokedToken => revokedToken.Events);
    }
}

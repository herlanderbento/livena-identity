using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

internal class UserCodeConfiguration : IEntityTypeConfiguration<UserCode>
{
    public void Configure(EntityTypeBuilder<UserCode> builder)
    {
        builder.ToTable("user_codes");

        builder.HasKey(userCode => userCode.Id);

        builder.Property(userCode => userCode.UserId).HasColumnName("user_id").IsRequired();

        builder
            .Property(userCode => userCode.Code)
            .HasColumnName("code")
            .HasMaxLength(12)
            .IsRequired();

        builder
            .Property(userCode => userCode.Purpose)
            .HasColumnName("purpose")
            .HasConversion(new SnakeCaseEnumToStringConverter<VerificationPurpose>())
            .IsRequired();

        builder.Property(userCode => userCode.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(userCode => userCode.ExpiresAt).HasColumnName("expires_at").IsRequired();

        builder.Property(userCode => userCode.UsedAt).HasColumnName("used_at");

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(userCode => userCode.UserId)
            .HasConstraintName("fk_user_codes_users")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(userCode => userCode.Events);
    }
}

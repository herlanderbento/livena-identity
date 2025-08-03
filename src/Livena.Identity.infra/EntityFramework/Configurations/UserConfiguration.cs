using Livena.Identity.Domain.Entity;
using Livena.Identity.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder
            .Property(user => user.Username)
            .HasColumnName("username")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.Email).HasColumnName("email").HasMaxLength(255);

        builder.Property(user => user.Phone).HasColumnName("phone").HasMaxLength(255);

        builder
            .Property(user => user.Password)
            .HasColumnName("password")
            .HasMaxLength(255)
            .IsRequired();

        builder
            .Property(user => user.Role)
            .HasColumnName("role")
            .HasConversion(new SnakeCaseEnumToStringConverter<Roles>())
            .IsRequired();

        builder.Property(user => user.IsVerified).HasColumnName("is_verified").IsRequired(false);

        builder.Property(user => user.IsActive).HasColumnName("is_active").IsRequired();

        builder.Property(user => user.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(user => user.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.Ignore(user => user.Events);
    }
}

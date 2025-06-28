using Livena.Identity.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

internal class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Username)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(255);
        
        builder.Property(user => user.Phone)
            .HasMaxLength(255);
        
        builder.Property(user => user.Password)
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Ignore(user => user.Events);
    }
}
using Livena.Identity.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Livena.Identity.infra.EntityFramework.Configurations;

public class RevokedTokenConfiguration: IEntityTypeConfiguration<RevokedToken>
{
    public void Configure(EntityTypeBuilder<RevokedToken> builder)
    {
        builder.HasKey(revokedToken => revokedToken.Id);
        
        builder.Property(revokedToken => revokedToken.UserId)
            .IsRequired();
        
        builder.Ignore(revokedToken => revokedToken.Events);
    }
}
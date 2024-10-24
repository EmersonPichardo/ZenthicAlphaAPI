using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class OAuthUserConfiguration : IEntityTypeConfiguration<OAuthUser>
{
    internal static readonly int AuthenticationTypeLength = 16;
    internal static readonly int UserNameLength = 64;
    internal static readonly int EmailLength = 64;

    public void Configure(EntityTypeBuilder<OAuthUser> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.OAuthUsers))
            .ConfigureKeys();

        builder.Property(entity => entity.AuthenticationType).HasMaxLength(AuthenticationTypeLength);
        builder.Property(entity => entity.UserName).HasMaxLength(UserNameLength);
        builder.Property(entity => entity.Email).HasMaxLength(EmailLength);
        builder.Property(entity => entity.Status).HasColumnType("varchar(50)").HasConversion<EnumToStringConverter<OAuthUserStatus>>();
    }
}

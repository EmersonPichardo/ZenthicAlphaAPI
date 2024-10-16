using Domain.Identity;
using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    internal static readonly int UserNameLength = 64;
    internal static readonly int EmailLength = 64;
    internal static readonly int PasswordLength = 88;
    internal static readonly int HashingStampLength = 152;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.Users))
            .ConfigureKeys()
            .ConfigureDeletionFilter();

        builder.Property(entity => entity.UserName).HasMaxLength(UserNameLength);
        builder.Property(entity => entity.Email).HasMaxLength(EmailLength);
        builder.Property(entity => entity.HashedPassword).HasMaxLength(PasswordLength);
        builder.Property(entity => entity.HashingStamp).HasMaxLength(HashingStampLength);
        builder.Property(entity => entity.Status).HasColumnType("varchar(50)").HasConversion<EnumToStringConverter<UserStatus>>();
    }
}

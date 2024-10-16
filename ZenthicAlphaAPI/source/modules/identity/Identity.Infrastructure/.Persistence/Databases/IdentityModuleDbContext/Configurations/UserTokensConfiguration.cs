using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Databases.IdentityDbContext.Configurations;

internal class UserTokensConfiguration : IEntityTypeConfiguration<UserToken>
{
    internal static readonly int TokenLength = 88;
    internal static readonly int HashingStampLength = 152;

    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder
            .ToTable(nameof(IdentityModuleDbContext.UserTokens))
            .ConfigureKeys();

        builder.Property(entity => entity.Token).HasMaxLength(TokenLength);
        builder.Property(entity => entity.HashingStamp).HasMaxLength(HashingStampLength);
    }
}

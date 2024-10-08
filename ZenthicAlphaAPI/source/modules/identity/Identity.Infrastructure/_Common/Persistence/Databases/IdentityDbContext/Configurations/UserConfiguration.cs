﻿using Identity.Application._Common.Persistence.Databases;
using Identity.Domain.User;
using Infrastructure.Persistence.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure._Common.Persistence.Databases.IdentityDbContext.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    internal static readonly int FullNameLength = 64;
    internal static readonly int EmailLength = 64;
    internal static readonly int PasswordLength = 128;
    internal static readonly int SaltLength = 32;
    internal static readonly int AlgorithmLength = 16;

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable(nameof(IIdentityDbContext.Users))
            .ConfigureKeys()
            .ConfigureDeletionFilter();

        builder.Property(entity => entity.FullName).HasMaxLength(FullNameLength);
        builder.Property(entity => entity.Email).HasMaxLength(EmailLength);
        builder.Property(entity => entity.Password).HasMaxLength(PasswordLength);
        builder.Property(entity => entity.Salt).HasMaxLength(SaltLength);
        builder.Property(entity => entity.Algorithm).HasMaxLength(AlgorithmLength);
        builder.Property(entity => entity.Status).HasColumnType("varchar(50)").HasConversion(
            valueTo => valueTo.ToString(),
            valueFrom => Enum.Parse<UserStatus>(valueFrom)
        );
    }
}

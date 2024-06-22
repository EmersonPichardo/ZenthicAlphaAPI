using Application._Common.Exceptions;
using Application._Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure._Persistence.Databases;

internal static class ModelBuilderExtensions
{
    internal static ModelBuilder MapToNormalizeMethod(this ModelBuilder modelBuilder)
    {
        var toNormalizeMethodName = nameof(StringExtensions.ToNormalize);
        var toNormalizeMethodInfo = typeof(StringExtensions).GetMethod(toNormalizeMethodName)
            ?? throw new NotFoundException($"Method {toNormalizeMethodName} was not found");

        modelBuilder
            .HasDbFunction(toNormalizeMethodInfo)
            .HasName("NormalizeVarchar");

        return modelBuilder;
    }
}

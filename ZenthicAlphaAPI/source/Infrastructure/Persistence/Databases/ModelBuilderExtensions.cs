using Application.Exceptions;
using Application.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Databases;

public static class ModelBuilderExtensions
{
    public static ModelBuilder MapToNormalizeMethod(this ModelBuilder modelBuilder)
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

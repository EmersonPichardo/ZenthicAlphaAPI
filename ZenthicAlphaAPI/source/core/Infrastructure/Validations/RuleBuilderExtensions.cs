﻿using Application.Auth;
using Domain.Entities.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Validations;

public static class RuleBuilderExtensions
{
    public static IRuleBuilderOptions<TModel, TProperty> Authenticated<TModel, TProperty>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder
    )
    {
        return ruleBuilder.Must((_, _, context)
            => context.RootContextData[nameof(IUserSession)] is AuthenticatedSession
        );
    }

    public static IRuleBuilderOptions<TModel, TProperty> Anonymous<TModel, TProperty>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder
    )
    {
        return ruleBuilder.Must((_, _, context)
            => context.RootContextData[nameof(IUserSession)] is null or AnonymousSession
        );
    }

    public static IRuleBuilderOptions<TModel, string> Url<TModel>(
        this IRuleBuilder<TModel, string> ruleBuilder
    )
    {
        return ruleBuilder.Must(property => Uri.TryCreate(property, UriKind.Absolute, out _));
    }

    public static IRuleBuilderOptions<TModel, TProperty> ExistAsync<TModel, TProperty, TEntity>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder,
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector
    )
        where TEntity : class, IEntity
        where TProperty : IEquatable<TProperty>
    {
        return ruleBuilder
            .MustAsync(
                async (property, cancellationToken)
                    => await ExistInSet(
                        entities,
                        selector,
                        property,
                        cancellationToken
                    )
            );
    }

    public static IRuleBuilderOptions<TModel, TProperty> NotExistAsync<TModel, TProperty, TEntity>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder,
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector
    )
        where TEntity : class, IEntity
        where TProperty : IEquatable<TProperty>
    {
        return ruleBuilder
            .MustAsync(
                async (property, cancellationToken)
                    => await NotExistInSet(
                        entities,
                        selector,
                        property,
                        cancellationToken
                    )
            );
    }

    public static IRuleBuilderOptions<TModel, TProperty> ExistIgnoringCurrentAsync<TModel, TProperty, TEntity>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder,
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector,
        Func<TModel, Guid> identifierSelector
    )
        where TEntity : class, IEntity
        where TProperty : IEquatable<TProperty>
    {
        return ruleBuilder
            .MustAsync(
                async (model, property, cancellationToken)
                    => await ExistInSetExcludingIdentifier(
                        entities,
                        identifierSelector(model),
                        selector,
                        property,
                        cancellationToken
                    )
            );
    }

    public static IRuleBuilderOptions<TModel, TProperty> NotExistIgnoringCurrentAsync<TModel, TProperty, TEntity>(
        this IRuleBuilder<TModel, TProperty> ruleBuilder,
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector,
        Func<TModel, Guid> identifierSelector
    )
        where TEntity : class, IEntity
        where TProperty : IEquatable<TProperty>
    {
        return ruleBuilder
            .MustAsync(
                async (model, property, cancellationToken)
                    => await NotExistInSetExcludingIdentifier(
                        entities,
                        identifierSelector(model),
                        selector,
                        property,
                        cancellationToken
                    )
            );
    }

    private static async Task<bool> ExistInSet<TProperty, TEntity>(
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector,
        TProperty property,
        CancellationToken cancellationToken = default
    )
        where TProperty : IEquatable<TProperty>
        where TEntity : class, IEntity
    {
        return await entities
            .AsNoTracking()
            .Select(selector)
            .AnyAsync(
                entityProperty => entityProperty.Equals(property),
                cancellationToken
            );
    }
    private static async Task<bool> NotExistInSet<TProperty, TEntity>(
        DbSet<TEntity> entities,
        Expression<Func<TEntity, TProperty>> selector,
        TProperty property,
        CancellationToken cancellationToken
    )
        where TProperty : IEquatable<TProperty>
        where TEntity : class, IEntity
    {
        return !await ExistInSet(
            entities,
            selector,
            property,
            cancellationToken
        );
    }
    private static async Task<bool> ExistInSetExcludingIdentifier<TProperty, TEntity>(
        DbSet<TEntity> entities,
        Guid identifier,
        Expression<Func<TEntity, TProperty>> selector,
        TProperty property,
        CancellationToken cancellationToken = default
    )
        where TProperty : IEquatable<TProperty>
        where TEntity : class, IEntity
    {
        var foundEntityProperty = await entities
            .Where(entity => entity.Id.Equals(identifier))
            .Select(selector)
            .FirstOrDefaultAsync(cancellationToken);

        if (foundEntityProperty?.Equals(property) ?? true)
            return false;

        return await ExistInSet(
            entities,
            selector,
            property,
            cancellationToken
        );
    }
    private static async Task<bool> NotExistInSetExcludingIdentifier<TProperty, TEntity>(
        DbSet<TEntity> entities,
        Guid identifier,
        Expression<Func<TEntity, TProperty>> selector,
        TProperty property,
        CancellationToken cancellationToken = default
    )
        where TProperty : IEquatable<TProperty>
        where TEntity : class, IEntity
    {
        return !await ExistInSetExcludingIdentifier(
            entities,
            identifier,
            selector,
            property,
            cancellationToken
        );
    }
}


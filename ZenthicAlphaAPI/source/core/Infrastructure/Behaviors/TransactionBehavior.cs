using Application.Commands;
using Application.Helpers;
using Application.Persistence.Databases;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using OneOf;

namespace Infrastructure.Behaviors;

internal class TransactionBehavior<TRequest, TResponse>(
    IEnumerable<IApplicationDbContext> dbContexts
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand
    where TResponse : IOneOf
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!CanGetUntrackedDbContexts(dbContexts, out var untrackedDbContexts))
            return await next();

        var transactions = new List<IDbContextTransaction>();

        foreach (var dbContext in untrackedDbContexts)
        {
            var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            transactions.Add(transaction);
        }

        try
        {
            var result = await next();

            if (result.IsSuccess())
                foreach (var transaction in transactions)
                    await transaction.CommitAsync(cancellationToken);

            if (result.IsFailure())
                foreach (var transaction in transactions)
                    await transaction.RollbackAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            foreach (var transaction in transactions)
                await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }

    private static bool CanGetUntrackedDbContexts(
        IEnumerable<IApplicationDbContext> dbContexts, out IList<IApplicationDbContext> untrackedDbContexts)
    {
        untrackedDbContexts = dbContexts
            .Where(dbContext => dbContext.Database.CurrentTransaction is null)
            .ToList();

        return untrackedDbContexts.Count > 0;
    }
}

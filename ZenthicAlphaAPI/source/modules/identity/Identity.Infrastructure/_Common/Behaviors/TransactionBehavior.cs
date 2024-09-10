using Application.Helpers;
using Identity.Application._Common.Persistence.Databases;
using MediatR;
using OneOf;

namespace Identity.Infrastructure._Common.Behaviors;

internal class TransactionBehavior<TRequest, TResponse>(
    IIdentityDbContext dbContext
)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : IOneOf
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (dbContext.Database.CurrentTransaction is not null)
            return await next().ConfigureAwait(false);

        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await next().ConfigureAwait(false);

            if (result.IsSuccess())
                await transaction.CommitAsync(cancellationToken);

            if (result.IsFailure())
                await transaction.RollbackAsync(cancellationToken);

            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);

            throw;
        }
    }
}

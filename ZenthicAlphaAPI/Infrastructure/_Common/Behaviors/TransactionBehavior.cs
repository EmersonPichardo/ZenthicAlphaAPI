using Application._Common.Persistence.Databases;
using Infrastructure._Common.Result;
using MediatR;
using OneOf;

namespace Infrastructure._Common.Behaviors;

internal class TransactionBehavior<TRequest, TResponse>(
    IApplicationDbContext dbContext
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

            if (result.Index == (int)ResultType.Success)
                await transaction.CommitAsync(cancellationToken);

            if (result.Index == (int)ResultType.Failure)
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

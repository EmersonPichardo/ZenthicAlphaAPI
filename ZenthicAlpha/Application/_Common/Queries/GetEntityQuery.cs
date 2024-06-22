namespace Application._Common.Queries;

public record GetEntityQuery<TResponse>
    : IGetEntityQuery
    , IQuery<TResponse>
    where TResponse : class
{
    public Guid Id { get; init; }
}
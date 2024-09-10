namespace Application.Queries;

public record GetEntityQuery<TResponse>
    : IGetEntityQuery
    , IQuery<TResponse>
{
    public Guid Id { get; init; }
}
using Application._Common.Failures;
using OneOf;

namespace Application._Common.Queries;

public record GetEntityQuery<TResponse>
    : IGetEntityQuery
    , IQuery<OneOf<TResponse, Failure>>
{
    public Guid Id { get; init; }
}
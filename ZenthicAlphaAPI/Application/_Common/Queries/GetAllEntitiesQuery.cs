namespace Application._Common.Queries;

public record GetAllEntitiesQuery<TResponse>
    : IQuery<IList<TResponse>>
where TResponse : class;

namespace Application.Queries;

public record GetAllEntitiesQuery<TResponse>
    : IQuery<IList<TResponse>>;

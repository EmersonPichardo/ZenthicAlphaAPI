namespace Application.Commands;

public record BaseDeleteCommand
    : IDeleteCommand
{
    public Guid Id { get; init; }
}

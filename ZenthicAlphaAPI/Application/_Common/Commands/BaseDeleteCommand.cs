namespace Application._Common.Commands;

public record BaseDeleteCommand
    : IDeleteCommand
{
    public Guid Id { get; init; }
}

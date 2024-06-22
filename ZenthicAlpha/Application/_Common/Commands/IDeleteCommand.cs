namespace Application._Common.Commands;

public interface IDeleteCommand
    : ICommand
{
    public Guid Id { get; init; }
}

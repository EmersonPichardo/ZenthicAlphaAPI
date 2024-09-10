namespace Application.Commands;

public interface IDeleteCommand
    : ICommand
{
    public Guid Id { get; init; }
}

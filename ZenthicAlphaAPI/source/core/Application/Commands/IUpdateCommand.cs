namespace Application.Commands;

public interface IUpdateCommand
    : ICommand
{
    public Guid Id { get; set; }
}

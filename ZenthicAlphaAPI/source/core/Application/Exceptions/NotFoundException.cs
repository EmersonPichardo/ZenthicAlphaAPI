namespace Application.Exceptions;

public class NotFoundException : Exception
{
    private const string baseMessage = "Resource was not found.";

    public NotFoundException()
        : base(baseMessage) { }

    public NotFoundException(Exception innerException)
        : base(baseMessage, innerException) { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }
}

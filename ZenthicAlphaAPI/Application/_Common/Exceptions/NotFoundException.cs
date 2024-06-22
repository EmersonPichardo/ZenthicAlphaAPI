namespace Application._Common.Exceptions;

public class NotFoundException : Exception
{
    public string? ResourceName { get; init; }
    public object? ResourceKey { get; init; }

    private const string baseMessage = "Resource was not found.";

    public NotFoundException()
        : base(baseMessage) { }

    public NotFoundException(Exception innerException)
        : base(baseMessage, innerException) { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException) { }

    public NotFoundException(string resourceName, object resourceKey)
        : base($"Resource {resourceName}{{{resourceKey}}} was not found.")
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }
    public NotFoundException(string resourceName, object resourceKey, Exception innerException)
        : base($"Resource {resourceName}{{{resourceKey}}} was not found.", innerException)
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }
}
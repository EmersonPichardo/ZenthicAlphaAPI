namespace Application.Failures;

public abstract record Failure
{
    public required string Title { get; init; }
    public string? Detail { get; init; }
    public IDictionary<string, object?> Extensions { get; init; } = new Dictionary<string, object?>();
}

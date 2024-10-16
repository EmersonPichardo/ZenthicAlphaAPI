namespace Application.Failures;

public abstract record Failure
{
    public required string Title { get; init; }
    public string? Detail { get; init; }
    public IDictionary<string, object?> Extensions { get; init; } = new Dictionary<string, object?>();

    public override string ToString() => $"{Title}. {Detail}";

    public Exception ToException()
    {
        var exception = new Exception(ToString());

        foreach (var extension in Extensions)
            exception.Data.Add(extension.Key, extension.Value);

        return exception;
    }
}

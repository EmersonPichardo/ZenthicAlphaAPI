using System.ComponentModel.DataAnnotations;

namespace Application.Behaviors;

public record OpenTelemetrySettings
{
    [Required]
    public required string IngestBaseEndpoint { get; init; }

    [Required]
    public required string LoggingPartialEndpoint { get; init; }

    [Required]
    public required string TracingPartialEndpoint { get; init; }

    [Required]
    public required string OtlpExportProtocol { get; init; }

    public string? ApiKey { get; init; }
}
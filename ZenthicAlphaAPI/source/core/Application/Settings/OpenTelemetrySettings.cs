using System.ComponentModel.DataAnnotations;

namespace Application.Settings;

public record OpenTelemetrySettings
{
    [Required]
    public required string IngestBaseEndpoint { get; init; }

    [Required]
    public required string OtlpExportProtocol { get; init; }

    public string? ApiKey { get; init; }
}
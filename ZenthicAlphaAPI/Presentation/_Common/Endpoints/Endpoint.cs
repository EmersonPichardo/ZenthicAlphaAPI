namespace Presentation._Common.Endpoints;

internal record Endpoint(
    HttpVerbose Verbose,
    string Route,
    Delegate Handler,
    int SuccessStatusCode,
    Type? SuccessType
);

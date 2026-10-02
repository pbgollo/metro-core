namespace Metro.Api.Health;

public sealed class HealthStatusResponse
{
    public string Status { get; init; } = string.Empty;
    public string TotalDuration { get; init; } = string.Empty;
    public IReadOnlyList<HealthCheckItemResponse> Checks { get; init; } = [];
}

public sealed class HealthCheckItemResponse
{
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Duration { get; init; } = string.Empty;
}

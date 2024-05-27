namespace Infrastructure.Api;

public static class DaprHealthCheckBuilderExtensions {
    public static IHealthChecksBuilder
        AddDapr(this IHealthChecksBuilder builder) =>
        builder.AddCheck<DaprHealthCheck>("dapr");
}
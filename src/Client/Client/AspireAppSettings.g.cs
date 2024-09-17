// This file is generated from the Aspire AppHost project. Rerun the Aspire AppHost
// to regenerate it.

public static class AspireAppSettings
{
    public static readonly Dictionary<string, string> Settings =
        new Dictionary<string, string>
        {
            ["DOTNET_SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION"] = "true",
            ["LOGGING:CONSOLE:FORMATTERNAME"] = "simple",
            ["LOGGING:CONSOLE:FORMATTEROPTIONS:TIMESTAMPFORMAT"] = "yyyy-MM-ddTHH:mm:ss.fffffff ",
            ["OTEL_BLRP_SCHEDULE_DELAY"] = "1000",
            ["OTEL_BSP_SCHEDULE_DELAY"] = "1000",
            ["OTEL_DOTNET_EXPERIMENTAL_ASPNETCORE_DISABLE_URL_QUERY_REDACTION"] = "true",
            ["OTEL_DOTNET_EXPERIMENTAL_HTTPCLIENT_DISABLE_URL_QUERY_REDACTION"] = "true",
            ["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES"] = "true",
            ["OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES"] = "true",
            ["OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY"] = "in_memory",
            ["OTEL_EXPORTER_OTLP_ENDPOINT"] = "https://localhost:21038",
            ["OTEL_EXPORTER_OTLP_HEADERS"] = "x-otlp-api-key=45662859ff5b72f65bd2e44ae651bd8c",
            ["OTEL_EXPORTER_OTLP_PROTOCOL"] = "grpc",
            ["OTEL_METRIC_EXPORT_INTERVAL"] = "1000",
            ["OTEL_METRICS_EXEMPLAR_FILTER"] = "trace_based",
            ["OTEL_RESOURCE_ATTRIBUTES"] = "service.instance.id=8155s7hj78",
            ["OTEL_SERVICE_NAME"] = "mauiclient",
            ["OTEL_TRACES_SAMPLER"] = "always_on",
            ["services:apiservice:https:0"] = "https://localhost:5002",
            ["SYSTEM_CONSOLE_ALLOW_ANSI_COLOR_REDIRECTION"] = "true",
        };
}
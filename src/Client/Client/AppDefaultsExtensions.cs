#if DEBUG
namespace Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This code is the client equivalent of the ServiceDefaults project. See https://aka.ms/dotnet/aspire/service-defaults
public static class AppDefaultsExtensions
{
	public static MauiAppBuilder AddAppDefaults(this MauiAppBuilder builder)
	{
		builder.ConfigureAppOpenTelemetry();

		builder.Services.AddServiceDiscovery();

		builder.Services.ConfigureHttpClientDefaults(http =>
		{
			// Turn on resilience by default
			http.AddStandardResilienceHandler();

			// Turn on service discovery by default
			http.AddServiceDiscovery();
		});

		builder.Services.TryAddEnumerable(
			ServiceDescriptor.Transient<IMauiInitializeService, OpenTelemetryInitializer>(_ => new OpenTelemetryInitializer()));

		return builder;
	}

	class OpenTelemetryInitializer : IMauiInitializeService
	{
		public void Initialize(IServiceProvider services)
		{
			services.GetService<MeterProvider>();
			services.GetService<TracerProvider>();
			services.GetService<LoggerProvider>();
		}
	}

	public static MauiAppBuilder ConfigureAppOpenTelemetry(this MauiAppBuilder builder)
	{
		builder.Logging.AddOpenTelemetry(logging =>
		{
			logging.IncludeFormattedMessage = true;
			logging.IncludeScopes = true;
		});

		builder.Services.AddOpenTelemetry()
			.WithMetrics(metrics =>
			{
				metrics.AddRuntimeInstrumentation()
					   .AddAppMeters();
			})
			.WithTracing(tracing =>
			{
#if DEBUG
				tracing.SetSampler(new AlwaysOnSampler());
#endif

				tracing
					   // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
					   //.AddGrpcClientInstrumentation()
					   .AddHttpClientInstrumentation();
			});

		builder.AddOpenTelemetryExporters();

		return builder;
	}

	private static void AddOpenTelemetryExporters(this MauiAppBuilder builder)
	{
		var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

		if (useOtlpExporter)
		{
			builder.SetOpenTelemetryEnvironmentVariables();

			builder.Services.AddOpenTelemetry().UseOtlpExporter();
		}
	}

	private static void SetOpenTelemetryEnvironmentVariables(this MauiAppBuilder builder)
	{
		var settings = builder.Configuration.AsEnumerable().Where(setting=>setting.Key.StartsWith("OTEL_"));
		foreach (var setting in settings)
		{
			Environment.SetEnvironmentVariable(setting.Key, setting.Value);
		}
	}

	private static void AddAppMeters(this MeterProviderBuilder meterProviderBuilder)
	{
		meterProviderBuilder.AddMeter("System.Net.Http");
	}
}
#endif
using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Infrastructure.Authentication;
using WorldExplorer.Common.Infrastructure.Authorization;
using WorldExplorer.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using StackExchange.Redis;

namespace WorldExplorer.Common.Infrastructure;

using Microsoft.Extensions.Configuration;

public static class InfrastructureConfiguration
{
	public static IServiceCollection AddAuthZ(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthorizationInternal();
		return services;
	}

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
		IConfiguration configuration,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string redisConnectionString)
    {
        services.AddAuthenticationInternal(configuration);

        services.AddAuthorizationInternal();
		
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        //NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        //services.TryAddSingleton(npgsqlDataSource);

        //services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
		
        services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

	    services.AddHybridCache();
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.AddMassTransit(configure =>
        {
            foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        //services
        //    .AddOpenTelemetry()
        //    .ConfigureResource(resource => resource.AddService(serviceName))
        //    .WithTracing(tracing =>
        //    {
        //        tracing
        //            .AddAspNetCoreInstrumentation()
        //            .AddHttpClientInstrumentation()
        //            .AddEntityFrameworkCoreInstrumentation()
        //            .AddRedisInstrumentation()
        //            .AddNpgsql()
        //            .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

        //        tracing.AddOtlpExporter();
        //    });

        return services;
    }
}

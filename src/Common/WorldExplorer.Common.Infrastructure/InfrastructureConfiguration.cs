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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public static class InfrastructureConfiguration
{
	public static IServiceCollection AddAuthZ(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddAuthorizationInternal();
		return services;
	}

    public static IHostApplicationBuilder AddInfrastructure(
        this IHostApplicationBuilder builder,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string redisConnectionString)
    {
        builder.Services.AddAuthenticationInternal(builder.Configuration);

        builder.Services.AddAuthorizationInternal();

        builder.Services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        builder.Services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        //NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        //services.TryAddSingleton(npgsqlDataSource);

        //services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();

        builder.Services.AddQuartz(configurator =>
        {
            var scheduler = Guid.NewGuid();
            configurator.SchedulerId = $"default-id-{scheduler}";
            configurator.SchedulerName = $"default-name-{scheduler}";
        });

        builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

	    builder.Services.AddHybridCache();
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            builder.Services.AddSingleton(connectionMultiplexer);
            builder.Services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            builder.Services.AddDistributedMemoryCache();
        }

		builder.AddRabbitMQClient("servicebus");
        builder.Services.AddMassTransit(configure =>
        {
			foreach (Action<IRegistrationConfigurator> configureConsumers in moduleConfigureConsumers)
            {
                configureConsumers(configure);
            }

            configure.SetKebabCaseEndpointNameFormatter();

            configure.UsingRabbitMq((context, cfg) =>
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

        return builder;
    }

    public static IHostApplicationBuilder AddDatabase<T>(this IHostApplicationBuilder builder,
	    string schema,
	    Action<SqlServerDbContextOptionsBuilder>? configure = null)
		where T:DbContext
    {
	    builder.Services.AddDbContextPool<T>((sp, options) =>
		                                                      options
			                                                      .UseSqlServer(
				                                                      builder.Configuration.GetConnectionString("Database"),
				                                                      optionsBuilder =>
				                                                      {
					                                                      optionsBuilder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema)
						                                                      .UseAzureSqlDefaults();
					                                                      configure?.Invoke(optionsBuilder);
					                                                      optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), [0]);
				                                                      })
			                                                      // todo remove
			                                                      .EnableDetailedErrors()
			                                                      .EnableSensitiveDataLogging()

			                                                      .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));
	    builder.EnrichSqlServerDbContext<T>();
	    return builder;
    }
}

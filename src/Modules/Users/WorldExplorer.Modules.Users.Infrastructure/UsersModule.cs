using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.Infrastructure.Database;
using WorldExplorer.Modules.Users.Infrastructure.Inbox;
using WorldExplorer.Modules.Users.Infrastructure.Outbox;
using WorldExplorer.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WorldExplorer.Modules.Users.Infrastructure;

using Application.Abstractions.Identity;
using Azure.Identity;
using Common.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
	    services.AddSingleton<IGraphClientService>(_ =>
	    {
		    var config = configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
		                              .Get<AzureAdB2CGraphClientConfiguration>();
		    ArgumentNullException.ThrowIfNull(config);
		    var clientSecretCredential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
		    return new GraphClientService(new GraphServiceClient(clientSecretCredential), config.DefaultApplicationId);
	    });

        services.AddDbContext<UsersDbContext>((sp, options) =>
            options
                .UseSqlServer(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

        services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

        //services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));

        //services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            //services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler =
                typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            //services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }
}

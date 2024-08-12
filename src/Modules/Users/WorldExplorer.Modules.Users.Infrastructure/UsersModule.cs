using WorldExplorer.Common.Application.EventBus;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Users.Domain.Users;
using WorldExplorer.Modules.Users.Infrastructure.Database;
using WorldExplorer.Modules.Users.Infrastructure.Inbox;
using WorldExplorer.Modules.Users.Infrastructure.Outbox;
using WorldExplorer.Modules.Users.Infrastructure.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WorldExplorer.Modules.Users.Infrastructure;

using Application.Abstractions.Identity;
using Azure.Identity;
using Common.Application.Abstractions.Data;
using Common.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Beta;

public static class UsersModule
{
    public static IHostApplicationBuilder AddUsersModule(
        this IHostApplicationBuilder builder)
    {
        builder.Services.AddDomainEventHandlers();

        builder.Services.AddIntegrationEventHandlers();

        builder.AddInfrastructure();

        builder.Services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        builder.Services.AddAuth(builder.Configuration);

        return builder;
    }

    private static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
	    builder.Services.AddSingleton<IGraphClientService>(_ =>
	    {
		    var config = builder.Configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
		                              .Get<AzureAdB2CGraphClientConfiguration>();
		    ArgumentNullException.ThrowIfNull(config);
		    var clientSecretCredential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
		    return new GraphClientService(new GraphServiceClient(clientSecretCredential), config.DefaultApplicationId);
	    });

        builder.AddDatabase<UsersDbContext>(Schemas.Users);

        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

        builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Users:Outbox"));

        //services.ConfigureOptions<ConfigureProcessOutboxJob>();

        builder.Services.Configure<InboxOptions>(builder.Configuration.GetSection("Users:Inbox"));

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
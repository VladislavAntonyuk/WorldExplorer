using WorldExplorer.Common.Infrastructure.Outbox;
using WorldExplorer.Common.Presentation.Endpoints;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WorldExplorer.Modules.Places.Infrastructure;

using Common.Application.Abstractions.Data;
using Database;
using Microsoft.EntityFrameworkCore;
using Places;

public static class PlacesModule
{
    public static IServiceCollection AddPlacesModule(
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
	    services.AddDbContext<PlacesDbContext>((sp, options) =>
            options
                .UseSqlServer(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Places)
                        .UseNetTopologySuite())
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IPlaceRepository, PlaceRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PlacesDbContext>());

        //services.Configure<OutboxOptions>(configuration.GetSection("Users:Outbox"));

        //services.ConfigureOptions<ConfigureProcessOutboxJob>();

        //services.Configure<InboxOptions>(configuration.GetSection("Users:Inbox"));

        //services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        //Type[] domainEventHandlers = Application.AssemblyReference.Assembly
        //    .GetTypes()
        //    .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
        //    .ToArray();

        //foreach (Type domainEventHandler in domainEventHandlers)
        //{
        //    services.TryAddScoped(domainEventHandler);

        //    Type domainEvent = domainEventHandler
        //        .GetInterfaces()
        //        .Single(i => i.IsGenericType)
        //        .GetGenericArguments()
        //        .Single();

        //    Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

        //    //services.Decorate(domainEventHandler, closedIdempotentHandler);
       // }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
       // Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
        //    .GetTypes()
        //    .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
        //    .ToArray();

        //foreach (Type integrationEventHandler in integrationEventHandlers)
        //{
        //    services.TryAddScoped(integrationEventHandler);

        //    Type integrationEvent = integrationEventHandler
        //        .GetInterfaces()
        //        .Single(i => i.IsGenericType)
        //        .GetGenericArguments()
        //        .Single();

        //    Type closedIdempotentHandler =
        //        typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

        //    //services.Decorate(integrationEventHandler, closedIdempotentHandler);
        //}
    }
}

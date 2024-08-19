namespace WorldExplorer.Modules.Travellers;

using Abstractions.Data;
using Application.Travellers.GetById;
using Application.Travellers.GetTravellers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.Modules.Travellers.Infrastructure.Database;

public static class TravellersModule
{
    public static IHostApplicationBuilder AddTravellersModule(
        this IHostApplicationBuilder builder)
    {
        //builder.Services.AddDomainEventHandlers();

       // builder.Services.AddIntegrationEventHandlers();

        builder.AddInfrastructure();

        return builder;
    }

    private static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDatabase<TravellersDbContext>(Schemas.Travellers);

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TravellersDbContext>());

        builder.Services.AddScoped<GetTravellersHandler>();
        builder.Services.AddScoped<GetTravellerByIdHandler>();

        builder.Services.AddGraphQLServer()
               .RegisterDbContextFactory<TravellersDbContext>()
               .AddQueryType<GetTravellersHandler>()
               .AddQueryType<GetTravellerByIdHandler>()
               .AddQueryConventions()
               .AddFiltering()
               .AddSorting();
    }

    public static IEndpointRouteBuilder MapTravellersEndpoint(this IEndpointRouteBuilder builder)
    {
	    builder.MapGraphQL();
	    return builder;
    }
}
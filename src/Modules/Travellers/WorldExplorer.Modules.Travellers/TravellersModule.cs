namespace WorldExplorer.Modules.Travellers;

using Abstractions.Data;
using Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.Common.Presentation.Endpoints;

public static class TravellersModule
{
    public static IHostApplicationBuilder AddTravellersModule(
        this IHostApplicationBuilder builder)
    {
        //builder.Services.AddDomainEventHandlers();

       // builder.Services.AddIntegrationEventHandlers();

        builder.AddInfrastructure();

        //builder.Services.AddEndpoints(AssemblyReference.Assembly);

        return builder;
    }

    private static void AddInfrastructure(this IHostApplicationBuilder builder)
    {
        builder.AddDatabase<TravellersDbContext>(Schemas.Travellers);

        //builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<TravellersDbContext>());

        builder.Services.AddScoped<TravellersService>();

        builder.Services.AddGraphQLServer()
               .RegisterDbContextFactory<TravellersDbContext>()
               .AddQueryType<TravellersService>()
               .AddQueryConventions()
               //.AddPagingArguments()
               .AddFiltering()
               //.AddProjections()
               .AddSorting()
               ;
        // .AddCatalogTypes()
        // .AddGraphQLConventions();
    }

    public static IEndpointRouteBuilder MapTravellersEndpoint(this IEndpointRouteBuilder builder)
    {
	    builder.MapGraphQL();
	    return builder;
    }
}
using System.Reflection;
using WorldExplorer.Api.Extensions;
using WorldExplorer.Api.Middleware;
using WorldExplorer.Common.Application;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.Common.Infrastructure.Configuration;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Users.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebApp.Infrastructure;
using OwaspHeaders.Core.Extensions;
using Scalar.AspNetCore;
using WebApp.Components;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();






builder.Services.AddBlazor();
builder.Services.AddWorldExplorerServices(builder.Configuration);
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddDatabase();









builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
	options.UseTransformer<BearerSecuritySchemeTransformer>();
});

Assembly[] moduleApplicationAssemblies = [
    WorldExplorer.Modules.Users.Application.AssemblyReference.Assembly];

builder.Services.AddApplication(moduleApplicationAssemblies);

string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
string redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");

builder.Services.AddInfrastructure(
	builder.Configuration,
    [
        //EventsModule.ConfigureConsumers(redisConnectionString),
        //TicketingModule.ConfigureConsumers,
        //AttendanceModule.ConfigureConsumers
    ],
    redisConnectionString);

builder.Services.AddHealthChecks()
    .AddSqlServer(databaseConnectionString)
    .AddRedis(redisConnectionString);

builder.Configuration.AddModuleConfiguration(["users"]);

builder.Services.AddUsersModule(builder.Configuration);

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();

	app.ApplyMigrations();
}


app.UseSecureHeadersMiddleware();
app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseLogContext();

//app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapEndpoints();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

public partial class Program;

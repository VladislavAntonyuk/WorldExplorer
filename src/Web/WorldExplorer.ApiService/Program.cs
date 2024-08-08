using System.Reflection;
using Scalar.AspNetCore;
using WorldExplorer.ApiService.Extensions;
using WorldExplorer.ApiService.Infrastructure;
using WorldExplorer.Common.Application;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Places.Infrastructure;
using WorldExplorer.Modules.Users.Infrastructure;
using WorldExplorer.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
	options.UseTransformer<BearerSecuritySchemeTransformer>();
});

Assembly[] moduleApplicationAssemblies = [
	WorldExplorer.Modules.Users.Application.AssemblyReference.Assembly];

builder.Services.AddApplication(moduleApplicationAssemblies);

builder.Services.AddInfrastructure(
	builder.Configuration,
	[
		//EventsModule.ConfigureConsumers(redisConnectionString),
		//TicketingModule.ConfigureConsumers,
		//AttendanceModule.ConfigureConsumers
	],
	"redisConnectionString");

builder.Configuration.AddModuleConfiguration(["users", "places", "traveller"]);

builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddPlacesModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
app.MapDefaultEndpoints();

app.Run();


namespace WorldExplorer.ApiService
{
	public partial class Program;
}
﻿using System.Reflection;
using Scalar.AspNetCore;
using WorldExplorer.ApiService.Extensions;
using WorldExplorer.ApiService.Infrastructure;
using WorldExplorer.Common.Application;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.Common.Presentation.Endpoints;
using WorldExplorer.Modules.Places.Infrastructure;
using WorldExplorer.Modules.Travellers;
using WorldExplorer.Modules.Users.Infrastructure;
using WorldExplorer.ServiceDefaults;
using AssemblyReference = WorldExplorer.Modules.Users.Application.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
	options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

Assembly[] moduleApplicationAssemblies =
[
	AssemblyReference.Assembly,
	WorldExplorer.Modules.Places.Application.AssemblyReference.Assembly,
	WorldExplorer.Modules.Travellers.AssemblyReference.Assembly
];

builder.Services.AddApplication(moduleApplicationAssemblies);

builder.AddInfrastructure([
	TravellersModule.ConfigureConsumers
]);

builder.Configuration.AddModuleConfiguration(["users", "places", "traveller"]);

builder.AddUsersModule();
builder.AddTravellersModule();
builder.AddPlacesModule();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	await app.ApplyMigrations();
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

app.Map("/", () => "World Explorer API. Created by Vladislav Antonyuk.");
app.MapEndpoints();
app.MapDefaultEndpoints();
app.MapGraphQL().RequireAuthorization();

await app.RunAsync();

public sealed partial class Program
{
	private Program()
	{
	}
}
﻿using Microsoft.EntityFrameworkCore;
using OwaspHeaders.Core.Extensions;
using WebApp.Components;
using WebApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazor();
builder.Services.AddWorldExplorerServices(builder.Configuration);
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddDatabase();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseSecureHeadersMiddleware();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

var db = app.Services.GetRequiredService<IDbContextFactory<WorldExplorerDbContext>>();
await using var dbContext = await db.CreateDbContextAsync();
await dbContext.Database.MigrateAsync();

await app.RunAsync();
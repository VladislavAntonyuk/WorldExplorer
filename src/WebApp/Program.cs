using Microsoft.EntityFrameworkCore;
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

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

var db = app.Services.GetRequiredService<IDbContextFactory<WorldExplorerDbContext>>();
using var dbContext = db.CreateDbContext();
dbContext.Database.Migrate();

app.Run();
using WebApp.Infrastructure;
using WebApp.Middleware;
using WorldExplorer.ServiceDefaults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();






builder.Services.AddBlazor();
builder.Services.AddWorldExplorerServices(builder.Configuration);
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddDatabase();









builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


string databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database");
string redisConnectionString = builder.Configuration.GetConnectionStringOrThrow("Cache");



builder.Services.AddHealthChecks()
    .AddSqlServer(databaseConnectionString)
    .AddRedis(redisConnectionString);



WebApplication app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{


	app.ApplyMigrations();
}


app.UseHttpsRedirection();



app.UseLogContext();

//app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapEndpoints();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.mapd
app.Run();

namespace WebApp
{
	public partial class Program;
}

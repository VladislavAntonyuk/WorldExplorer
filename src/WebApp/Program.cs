using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Infrastructure;
using WebApp.Services.AI;
using WebApp.Services.Image;
using WebApp.Services.Place;
using WebApp.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddTranslations();
builder.Services.AddLogging();

builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetRequiredSection("OpenAI"));
builder.Services.AddSingleton<IAiService, AiService>();

builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
builder.Services.AddScoped<IImageSearchService, ImageSearchService>();

builder.Services.AddAuth(builder.Configuration);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IGraphClientService>(_ =>
{
	var config = builder.Configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
							  .Get<AzureAdB2CGraphClientConfiguration>();
	ArgumentNullException.ThrowIfNull(config);
	var clientSecretCredential = new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
	return new GraphClientService(new GraphServiceClient(clientSecretCredential), config.DefaultApplicationId);
});

builder.Services.AddPooledDbContextFactory<WorldExplorerDbContext>(opt =>
{
	opt.UseSqlite("Data Source=WorldExplorer.db", optionsBuilder => optionsBuilder.UseNetTopologySuite())
	   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
});

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
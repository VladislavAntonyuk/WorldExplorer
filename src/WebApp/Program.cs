using Azure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Identity;
using WebApp.Infrastructure;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
builder.Services.AddScoped<IImageSearchService, ImageSearchService>();
builder.Services.AddScoped(_ =>
{
	var config = builder.Configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
							  .Get<AzureAdB2CGraphClientConfiguration>();
	ArgumentNullException.ThrowIfNull(config);
	var clientSecretCredential =
		new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
	return new GraphServiceClient(clientSecretCredential);
});
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetRequiredSection("OpenAI"));
builder.Services.AddSingleton<IAiService, AiService>();
builder.Services.AddLogging();
builder.Services.AddPooledDbContextFactory<WorldExplorerDbContext>(opt =>
{
	opt.UseSqlite("Data Source=WorldExplorer.db", optionsBuilder => optionsBuilder.UseNetTopologySuite())
	   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
});
builder.Services.AddTranslations();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

var db = app.Services.GetRequiredService<IDbContextFactory<WorldExplorerDbContext>>();
using var dbContext = db.CreateDbContext();
dbContext.Database.Migrate();
app.Run();

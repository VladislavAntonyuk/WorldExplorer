using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Beta;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using OwaspHeaders.Core.Extensions;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using WebApp.Infrastructure;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	   .AddMicrosoftIdentityWebApp(options => builder.Configuration.Bind("AzureAdB2C", options));
builder.Services.AddAuthentication().AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAdB2C");
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("GoogleImages", client => client.BaseAddress = new Uri("https://serpapi.com"));
builder.Services.AddScoped<IImageSearchService, ImageSearchService>();
builder.Services.AddScoped(_ => new GraphServiceClient(new DefaultAzureCredential()));
builder.Services.AddScoped<IGraphClientService, GraphClientService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.Configure<OpenAiSettings>(builder.Configuration.GetRequiredSection("OpenAI"));
builder.Services.AddSingleton<IAiService, AiService>();
builder.Services.AddLogging();
builder.Services.AddPooledDbContextFactory<WorldExplorerDbContext>(opt => opt.UseSqlite("Data Source=WorldExplorer.db")
																			 .UseQueryTrackingBehavior(
																				 QueryTrackingBehavior
																					 .NoTrackingWithIdentityResolution));
builder.Services.AddI18nText();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSecureHeadersMiddleware();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var db = app.Services.GetRequiredService<IDbContextFactory<WorldExplorerDbContext>>();
using var dbContext = db.CreateDbContext();
dbContext.Database.Migrate();
app.Run();
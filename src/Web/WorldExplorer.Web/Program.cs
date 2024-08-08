using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using WebApp.Infrastructure;
using WorldExplorer.Common.Infrastructure;
using WorldExplorer.ServiceDefaults;
using WorldExplorer.Web;
using WorldExplorer.Web.Components;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.Services.AddBlazor();
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddAuthZ(builder.Configuration);

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, Microsoft.Identity.Web.Constants.AzureAdB2C)
       .EnableTokenAcquisitionToCallDownstreamApi()
       .AddDownstreamApi("WorldExplorerApiClient", builder.Configuration.GetSection("WorldExplorerApiClient"))
       .AddInMemoryTokenCaches();

builder.Services.AddHttpClient<ApiService>(s =>
       {
	       s.BaseAddress = new Uri("https+http://apiservice");
       })
       .AddMicrosoftIdentityUserAuthenticationHandler(nameof(ApiService), builder.Configuration.GetSection("WorldExplorerApiClient"));

builder.Services.AddControllersWithViews(options =>
{
	var policy = new AuthorizationPolicyBuilder()
				 .RequireAuthenticatedUser()
				 .Build();
	options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();



builder.Services.AddTransient<WorldExplorerApiClient>();
builder.Services.AddHttpContextAccessor();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler(); ;


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();

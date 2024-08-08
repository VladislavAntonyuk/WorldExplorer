using System.IdentityModel.Tokens.Jwt;
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
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddWorldExplorerServices(builder.Configuration);



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

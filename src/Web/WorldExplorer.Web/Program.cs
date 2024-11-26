using WorldExplorer.ServiceDefaults;
using WorldExplorer.Web.Components;
using WorldExplorer.Web.User;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");
builder.AddRedisDistributedCache("cache");

builder.Services.AddBlazor();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddUserServices(builder.Configuration);
builder.Services.AddWorldExplorerServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.UseAntiforgery();

app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

await app.RunAsync();
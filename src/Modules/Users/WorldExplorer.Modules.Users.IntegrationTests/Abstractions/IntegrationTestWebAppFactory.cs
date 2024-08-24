namespace WorldExplorer.Modules.Users.IntegrationTests.Abstractions;

using ApiService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.Redis;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    //private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
    //    .WithImage("postgres:latest")
    //    .WithDatabase("WorldExplorer")
    //    .WithUsername("postgres")
    //    .WithPassword("postgres")
    //    .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    //private readonly KeycloakContainer _keycloakContainer = new KeycloakBuilder()
    //    .WithImage("quay.io/keycloak/keycloak:latest")
    //    .WithResourceMapping(
    //        new FileInfo("WorldExplorer-realm-export.json"),
    //        new FileInfo("/opt/keycloak/data/import/realm.json"))
    //    .WithCommand("--import-realm")
    //    .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //Environment.SetEnvironmentVariable("ConnectionStrings:Database", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ConnectionStrings:Cache", _redisContainer.GetConnectionString());

      //  string keycloakAddress = _keycloakContainer.GetBaseAddress();
        //string keyCloakRealmUrl = $"{keycloakAddress}realms/WorldExplorer";

        //Environment.SetEnvironmentVariable(
        //    "Authentication:MetadataAddress",
        //    $"{keyCloakRealmUrl}/.well-known/openid-configuration");
        //Environment.SetEnvironmentVariable(
        //    "Authentication:TokenValidationParameters:ValidIssuer",
        //    keyCloakRealmUrl);

        //builder.ConfigureTestServices(services =>
        //{
        //    services.Configure<KeyCloakOptions>(o =>
        //    {
        //  //      o.AdminUrl = $"{keycloakAddress}admin/realms/WorldExplorer/";
        //        o.TokenUrl = $"{keyCloakRealmUrl}/protocol/openid-connect/token";
        //    });
        //});
    }

    public async Task InitializeAsync()
    {
     //   await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
       // await _keycloakContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        //await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        //await _keycloakContainer.StopAsync();
    }
}

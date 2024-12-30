namespace WorldExplorer.Modules.Users.Infrastructure;

using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Azure.Identity;
using Common.Application.Messaging;
using Common.Infrastructure;
using Common.Presentation.Endpoints;
using Database;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph.Beta;
using Outbox;
using Presentation;
using Users;

public static class UsersModule
{
	public static IHostApplicationBuilder AddUsersModule(this IHostApplicationBuilder builder)
	{
		builder.Services.AddDomainEventHandlers();

		builder.AddInfrastructure();

		builder.Services.AddEndpoints(AssemblyReference.Assembly);

		builder.Services.AddAuth(builder.Configuration);

		return builder;
	}

	private static void AddInfrastructure(this IHostApplicationBuilder builder)
	{
		builder.Services.AddSingleton<IGraphClientService>(_ =>
		{
			var config = builder.Configuration.GetRequiredSection(AzureAdB2CGraphClientConfiguration.ConfigurationName)
								.Get<AzureAdB2CGraphClientConfiguration>();
			ArgumentNullException.ThrowIfNull(config);
			var clientSecretCredential =
				new ClientSecretCredential(config.TenantId, config.ClientId, config.ClientSecret);
			return new GraphClientService(new GraphServiceClient(clientSecretCredential), config.DefaultApplicationId);
		});

		builder.AddDatabase<UsersDbContext>(Schemas.Users, optionsBuilder =>
		{
			optionsBuilder.UseAsyncSeeding(async (dbContext, _, cancellationToken) =>
			{
				var userGuid = Guid.Parse("19d3b2c7-8714-4851-ac73-95aeecfba3a6");
				var user = dbContext.Find<User>(userGuid);
				if (user != null)
				{
					return;
				}

				dbContext.Add(User.Create(userGuid, new UserSettings()));
				await dbContext.SaveChangesAsync(cancellationToken);
			});
		});

		builder.Services.AddScoped<IUserRepository, UserRepository>();

		builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

		builder.Services.Configure<OutboxOptions>(builder.Configuration.GetSection("Users:Outbox"));

		builder.Services.ConfigureOptions<ConfigureProcessOutboxJob>();
	}

	private static void AddDomainEventHandlers(this IServiceCollection services)
	{
		var domainEventHandlers = Application.AssemblyReference.Assembly.GetTypes()
		                                     .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)));

		foreach (var domainEventHandler in domainEventHandlers)
		{
			services.AddKeyedScoped(typeof(IDomainEventHandler), GetKey(domainEventHandler), domainEventHandler);
		}
		
		static string GetKey(Type type)
		{
			const int handlerNameSuffixLength = 7;
			return type.Name.AsSpan(..^handlerNameSuffixLength).ToString();
		}
	}
}
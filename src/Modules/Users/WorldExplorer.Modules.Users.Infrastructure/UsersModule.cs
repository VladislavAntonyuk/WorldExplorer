namespace WorldExplorer.Modules.Users.Infrastructure;

using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Azure.Identity;
using Common.Application.EventBus;
using Common.Application.Messaging;
using Common.Infrastructure;
using Common.Presentation.Endpoints;
using Database;
using Domain.Users;
using Inbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

		builder.Services.AddIntegrationEventHandlers();

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

		builder.Services.Configure<InboxOptions>(builder.Configuration.GetSection("Users:Inbox"));

		builder.Services.ConfigureOptions<ConfigureProcessInboxJob>();
	}

	private static void AddDomainEventHandlers(this IServiceCollection services)
	{
		var domainEventHandlers = Application.AssemblyReference.Assembly.GetTypes()
		                                     .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
		                                     .ToArray();

		foreach (var domainEventHandler in domainEventHandlers)
		{
			services.TryAddScoped(domainEventHandler);

			var domainEvent = domainEventHandler.GetInterfaces()
			                                    .Single(i => i.IsGenericType)
			                                    .GetGenericArguments()
			                                    .Single();

			var closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

			services.Decorate(domainEventHandler, closedIdempotentHandler);
		}
	}

	private static void AddIntegrationEventHandlers(this IServiceCollection services)
	{
		var integrationEventHandlers = AssemblyReference.Assembly.GetTypes()
		                                                .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
		                                                .ToArray();

		foreach (var integrationEventHandler in integrationEventHandlers)
		{
			services.TryAddScoped(integrationEventHandler);

			var integrationEvent = integrationEventHandler.GetInterfaces()
			                                              .Single(i => i.IsGenericType)
			                                              .GetGenericArguments()
			                                              .Single();

			var closedIdempotentHandler = typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

			services.Decorate(integrationEventHandler, closedIdempotentHandler);
		}
	}
}
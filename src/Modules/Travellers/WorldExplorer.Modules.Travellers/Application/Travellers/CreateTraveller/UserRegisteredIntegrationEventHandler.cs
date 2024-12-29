namespace WorldExplorer.Modules.Travellers.Application.Travellers.CreateTraveller;

using Common.Application.EventBus;
using Common.Application.Exceptions;
using MediatR;
using Users.IntegrationEvents;

internal sealed class UserRegisteredIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<UserRegisteredIntegrationEvent>
{
	public override async Task Handle(UserRegisteredIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(new CreateTravellerCommand(integrationEvent.UserId, integrationEvent.Name), cancellationToken);

		if (result.IsFailure)
		{
			throw new WorldExplorerException(nameof(CreateTravellerCommand), result.Error);
		}
	}
}
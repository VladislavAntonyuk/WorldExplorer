namespace WorldExplorer.Modules.Travellers.Application.Travellers.DeleteTraveller;

using Application.Travellers.DeleteTraveller;
using Common.Application.EventBus;
using Common.Application.Exceptions;
using MediatR;
using Users.IntegrationEvents;

internal sealed class UserDeletedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<UserDeletedIntegrationEvent>
{
	public override async Task Handle(UserDeletedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(new DeleteTravellerCommand(integrationEvent.UserId), cancellationToken);

		if (result.IsFailure)
		{
			throw new WorldExplorerException(nameof(DeleteTravellerCommand), result.Error);
		}
	}
}
namespace WorldExplorer.Modules.Travellers.Application.Visits.DeletePlace;

using Application.Travellers.DeleteTraveller;
using Application.Visits.DeletePlace;
using Common.Application.EventBus;
using Common.Application.Exceptions;
using MediatR;
using Places.IntegrationEvents;

internal sealed class PlaceDeletedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<PlaceDeletedIntegrationEvent>
{
	public override async Task Handle(PlaceDeletedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(new DeletePlaceCommand(integrationEvent.PlaceId), cancellationToken);

		if (result.IsFailure)
		{
			throw new WorldExplorerException(nameof(DeletePlaceCommand), result.Error);
		}
	}
}
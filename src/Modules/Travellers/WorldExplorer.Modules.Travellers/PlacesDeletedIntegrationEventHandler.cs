namespace WorldExplorer.Modules.Travellers;

using Application.Travellers.CreateTraveller;
using Application.Visits.DeletePlaces;
using Common.Application.EventBus;
using Common.Application.Exceptions;
using MediatR;
using Places.IntegrationEvents;

internal sealed class PlacesDeletedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<PlacesDeletedIntegrationEvent>
{
	public override async Task Handle(PlacesDeletedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(new DeletePlacesCommand(), cancellationToken);

		if (result.IsFailure)
		{
			throw new WorldExplorerException(nameof(DeleteTravellerCommand), result.Error);
		}
	}
}
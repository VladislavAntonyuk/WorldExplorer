namespace WorldExplorer.Modules.Travellers.Application.Visits.CreatePlace;

using Application.Visits.CreatePlace;
using Common.Application.EventBus;
using Common.Application.Exceptions;
using MediatR;
using Places.IntegrationEvents;

internal sealed class PlaceCreatedIntegrationEventHandler(ISender sender)
	: IntegrationEventHandler<PlaceCreatedIntegrationEvent>
{
	public override async Task Handle(PlaceCreatedIntegrationEvent integrationEvent,
		CancellationToken cancellationToken = default)
	{
		var result = await sender.Send(new CreatePlaceCommand(integrationEvent.PlaceId), cancellationToken);

		if (result.IsFailure)
		{
			throw new WorldExplorerException(nameof(CreatePlaceCommand), result.Error);
		}
	}
}
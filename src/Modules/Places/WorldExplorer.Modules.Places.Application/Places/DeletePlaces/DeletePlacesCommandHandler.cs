namespace WorldExplorer.Modules.Places.Application.Places.DeletePlaces;

using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;
using IntegrationEvents;
using WorldExplorer.Common.Application.EventBus;

internal sealed class DeletePlacesCommandHandler(IPlaceRepository placeRepository, IEventBus eventBus)
	: ICommandHandler<DeletePlacesCommand>
{
	public async Task<Result> Handle(DeletePlacesCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.Clear(cancellationToken);
		await eventBus.PublishAsync(new PlacesDeletedIntegrationEvent(Guid.CreateVersion7(), DateTime.UtcNow), cancellationToken);
		return Result.Success();
	}
}
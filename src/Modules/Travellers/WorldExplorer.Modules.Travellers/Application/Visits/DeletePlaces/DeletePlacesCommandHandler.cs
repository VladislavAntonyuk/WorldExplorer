namespace WorldExplorer.Modules.Travellers.Application.Visits.DeletePlaces;

using Common.Application.Messaging;
using Common.Domain;

internal sealed class DeletePlacesCommandHandler(IPlaceRepository placeRepository)
	: ICommandHandler<DeletePlacesCommand>
{
	public async Task<Result> Handle(DeletePlacesCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.DeleteAsync(cancellationToken);

		return Result.Success();
	}
}
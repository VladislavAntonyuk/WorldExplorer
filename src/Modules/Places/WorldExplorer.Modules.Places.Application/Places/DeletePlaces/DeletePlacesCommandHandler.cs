namespace WorldExplorer.Modules.Places.Application.Places.DeletePlaces;

using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;

internal sealed class DeletePlacesCommandHandler(IPlaceRepository placeRepository)
	: ICommandHandler<DeletePlacesCommand>
{
	public async Task<Result> Handle(DeletePlacesCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.Clear(cancellationToken);
		return Result.Success();
	}
}
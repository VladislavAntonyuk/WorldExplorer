namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Abstractions.Data;
using GetPlace;
using NetTopologySuite.Geometries;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Places.Domain.Places;
using Location = Abstractions.Location;

internal sealed class DeletePlaceCommandHandler(IPlaceRepository placeRepository)
	: ICommandHandler<DeletePlaceCommand>
{
	public async Task<Result> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
	{
		var place = await placeRepository.GetAsync(request.Id, cancellationToken);
		if (place is null)
		{
			return Result.Failure(PlaceErrors.NotFound(request.Id));
		}

		await placeRepository.Delete(request.Id, cancellationToken);
		return Result.Success();
	}
}
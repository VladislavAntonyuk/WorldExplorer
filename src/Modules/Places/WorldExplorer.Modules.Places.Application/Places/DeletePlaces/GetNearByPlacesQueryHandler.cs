namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Abstractions.Data;
using GetPlace;
using NetTopologySuite.Geometries;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Places.Domain.Places;
using Location = Abstractions.Location;

internal sealed class DeletePlacesCommandHandler(IPlaceRepository placeRepository)
	: ICommandHandler<DeletePlacesCommand>
{
	public async Task<Result> Handle(DeletePlacesCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.Clear(cancellationToken);
		return Result.Success();
	}
}
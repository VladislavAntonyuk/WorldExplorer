namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using Abstractions.Data;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Places.Domain.Places;

internal sealed class UpdatePlaceCommandHandler(IPlaceRepository placeRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<UpdatePlaceCommand>
{
	public async Task<Result> Handle(UpdatePlaceCommand request, CancellationToken cancellationToken)
	{
		var place = await placeRepository.GetAsync(request.Id, cancellationToken);
		if (place is null)
		{
			return Result.Failure(PlaceErrors.NotFound(request.Id));
		}

		place.Update(request.PlaceRequest.Name, request.PlaceRequest.Location.ToPoint(), request.PlaceRequest.Description);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
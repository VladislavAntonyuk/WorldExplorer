namespace WorldExplorer.Modules.Places.Application.Places.UpdatePlace;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;

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

		place.Update(
			request.PlaceRequest.Name,
			request.PlaceRequest.Location.ToPoint(),
			request.PlaceRequest.Description,
			place.Images);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
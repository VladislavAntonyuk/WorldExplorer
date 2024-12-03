namespace WorldExplorer.Modules.Places.Application.Places.DeletePlace;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;

internal sealed class DeletePlaceCommandHandler(IPlaceRepository placeRepository, IUnitOfWork unitOfWork) : ICommandHandler<DeletePlaceCommand>
{
	public async Task<Result> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
	{
		var place = await placeRepository.GetAsync(request.Id, cancellationToken);
		if (place is null)
		{
			return Result.Failure(PlaceErrors.NotFound(request.Id));
		}

		place.Delete();
		placeRepository.Delete(place);
		await unitOfWork.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
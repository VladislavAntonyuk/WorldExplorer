namespace WorldExplorer.Modules.Places.Application.Places.DeletePlace;

using Common.Application.Messaging;
using Common.Domain;
using Domain.Places;

internal sealed class DeletePlaceCommandHandler(IPlaceRepository placeRepository) : ICommandHandler<DeletePlaceCommand>
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
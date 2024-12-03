namespace WorldExplorer.Modules.Travellers.Application.Visits.DeletePlaces;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;
using CreatePlace;

internal sealed class DeletePlacesCommandHandler(IPlaceRepository placeRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<DeletePlacesCommand>
{
	public async Task<Result> Handle(DeletePlacesCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.DeleteAsync(cancellationToken);

		return Result.Success();
	}
}
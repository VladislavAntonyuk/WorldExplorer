namespace WorldExplorer.Modules.Travellers.Application.Visits.DeletePlace;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;

internal sealed class DeletePlaceCommandHandler(IPlaceRepository placeRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<DeletePlaceCommand>
{
	public async Task<Result> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
	{
		await placeRepository.DeleteAsync(request.Id, cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
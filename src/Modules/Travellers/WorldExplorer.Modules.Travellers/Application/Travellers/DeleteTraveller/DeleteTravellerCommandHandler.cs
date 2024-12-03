namespace WorldExplorer.Modules.Travellers.Application.Travellers.DeleteTraveller;

using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Travellers.Abstractions.Data;

internal sealed class DeleteTravellerCommandHandler(ITravellerRepository travellerRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<DeleteTravellerCommand>
{
	public async Task<Result> Handle(DeleteTravellerCommand request, CancellationToken cancellationToken)
	{
		await travellerRepository.DeleteAsync(request.TravellerId, cancellationToken);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
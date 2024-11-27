namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.DeleteLocationInfoRequest;

using Common.Application.Messaging;
using Common.Domain;
using Domain.LocationInfo;

internal sealed class DeleteLocationInfoRequestCommandHandler(ILocationInfoRepository placeRepository)
	: ICommandHandler<DeleteLocationInfoRequestCommand>
{
	public async Task<Result> Handle(DeleteLocationInfoRequestCommand request, CancellationToken cancellationToken)
	{
		var place = await placeRepository.GetAsync(request.Id, cancellationToken);
		if (place is null)
		{
			return Result.Failure(LocationInfoRequestErrors.NotFound(request.Id));
		}

		await placeRepository.Delete(request.Id, cancellationToken);
		return Result.Success();
	}
}
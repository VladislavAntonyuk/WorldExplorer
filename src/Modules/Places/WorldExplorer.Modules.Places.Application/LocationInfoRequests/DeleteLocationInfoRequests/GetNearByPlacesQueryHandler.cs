namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.DeletePlace;

using Places.GetPlaces;
using WorldExplorer.Common.Application.Messaging;
using WorldExplorer.Common.Domain;
using WorldExplorer.Modules.Places.Domain.LocationInfo;

internal sealed class DeleteLocationInfoRequestsCommandHandler(ILocationInfoRepository locationInfoRepository)
	: ICommandHandler<DeleteLocationInfoRequestsCommand>
{
	public async Task<Result> Handle(DeleteLocationInfoRequestsCommand request, CancellationToken cancellationToken)
	{
		await locationInfoRepository.Clear(cancellationToken);
		return Result.Success();
	}
}
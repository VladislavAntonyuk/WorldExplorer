namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.DeleteLocationInfoRequests;

using Common.Application.Messaging;
using Common.Domain;
using Domain.LocationInfo;

internal sealed class DeleteLocationInfoRequestsCommandHandler(ILocationInfoRepository locationInfoRepository)
	: ICommandHandler<DeleteLocationInfoRequestsCommand>
{
	public async Task<Result> Handle(DeleteLocationInfoRequestsCommand request, CancellationToken cancellationToken)
	{
		await locationInfoRepository.Clear(cancellationToken);
		return Result.Success();
	}
}
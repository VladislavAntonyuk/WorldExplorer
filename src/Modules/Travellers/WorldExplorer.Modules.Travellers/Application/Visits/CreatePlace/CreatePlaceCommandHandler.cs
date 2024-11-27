namespace WorldExplorer.Modules.Travellers.Application.Visits.CreatePlace;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;

internal sealed class CreatePlaceCommandHandler(IPlaceRepository placeRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<CreatePlaceCommand>
{
	public async Task<Result> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
	{
		var place = new Place
		{
			Id = request.Id
		};

		placeRepository.Insert(place);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
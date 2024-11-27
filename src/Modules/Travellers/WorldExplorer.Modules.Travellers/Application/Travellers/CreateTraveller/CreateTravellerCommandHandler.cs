namespace WorldExplorer.Modules.Travellers.Application.Travellers.CreateTraveller;

using Abstractions.Data;
using Common.Application.Messaging;
using Common.Domain;

internal sealed class CreateTravellerCommandHandler(ITravellerRepository travellerRepository, IUnitOfWork unitOfWork)
	: ICommandHandler<CreateTravellerCommand>
{
	public async Task<Result> Handle(CreateTravellerCommand request, CancellationToken cancellationToken)
	{
		var traveller = Traveller.Create(request.TravellerId, []);

		travellerRepository.Insert(traveller);

		await unitOfWork.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
namespace WorldExplorer.Modules.Travellers.Application.Travellers.CreateTraveller;

using Common.Application.Messaging;

public sealed record CreateTravellerCommand(Guid TravellerId) : ICommand;
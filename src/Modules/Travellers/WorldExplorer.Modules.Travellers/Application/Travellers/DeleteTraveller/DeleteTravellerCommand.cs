namespace WorldExplorer.Modules.Travellers.Application.Travellers.CreateTraveller;

using Common.Application.Messaging;

public sealed record DeleteTravellerCommand(Guid TravellerId) : ICommand;
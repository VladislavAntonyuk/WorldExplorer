namespace WorldExplorer.Modules.Travellers.Application.Travellers.DeleteTraveller;

using Common.Application.Messaging;

public sealed record DeleteTravellerCommand(Guid TravellerId) : ICommand;
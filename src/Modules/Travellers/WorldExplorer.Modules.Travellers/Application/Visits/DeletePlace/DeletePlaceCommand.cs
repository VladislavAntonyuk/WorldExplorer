namespace WorldExplorer.Modules.Travellers.Application.Visits.DeletePlace;

using Common.Application.Messaging;

public sealed record DeletePlaceCommand(Guid Id) : ICommand;
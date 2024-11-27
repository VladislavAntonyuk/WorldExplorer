namespace WorldExplorer.Modules.Travellers.Application.Visits.CreatePlace;

using Common.Application.Messaging;

public sealed record CreatePlaceCommand(Guid Id) : ICommand;
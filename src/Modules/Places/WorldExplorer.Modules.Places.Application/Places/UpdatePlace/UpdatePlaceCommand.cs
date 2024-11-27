namespace WorldExplorer.Modules.Places.Application.Places.UpdatePlace;

using Common.Application.Messaging;
using GetPlace;

public sealed record UpdatePlaceCommand(Guid Id, PlaceRequest PlaceRequest) : ICommand;
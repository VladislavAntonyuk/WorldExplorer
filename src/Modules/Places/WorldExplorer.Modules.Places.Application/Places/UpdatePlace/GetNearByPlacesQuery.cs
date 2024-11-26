namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using GetPlace;
using WorldExplorer.Common.Application.Messaging;

public sealed record UpdatePlaceCommand(Guid Id, PlaceRequest PlaceRequest) : ICommand;
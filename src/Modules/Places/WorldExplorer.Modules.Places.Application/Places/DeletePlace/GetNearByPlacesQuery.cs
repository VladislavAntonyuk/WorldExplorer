namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using GetPlace;
using WorldExplorer.Common.Application.Messaging;

public sealed record DeletePlaceCommand(Guid Id) : ICommand;
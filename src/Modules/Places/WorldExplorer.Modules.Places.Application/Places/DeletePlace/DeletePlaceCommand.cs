namespace WorldExplorer.Modules.Places.Application.Places.DeletePlace;

using Common.Application.Messaging;

public sealed record DeletePlaceCommand(Guid Id) : ICommand;
namespace WorldExplorer.Modules.Places.Application.Places.GetPlaces;

using WorldExplorer.Common.Application.Messaging;

public sealed record DeleteLocationInfoRequestCommand(int Id) : ICommand;
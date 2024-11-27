namespace WorldExplorer.Modules.Places.Application.LocationInfoRequests.DeleteLocationInfoRequest;

using Common.Application.Messaging;

public sealed record DeleteLocationInfoRequestCommand(int Id) : ICommand;
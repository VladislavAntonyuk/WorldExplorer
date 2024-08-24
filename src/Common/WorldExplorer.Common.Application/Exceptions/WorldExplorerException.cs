namespace WorldExplorer.Common.Application.Exceptions;

using Domain;

public sealed class WorldExplorerException(
	string requestName,
	Error? error = default,
	Exception? innerException = default) : Exception("Application exception", innerException)
{
	public string RequestName { get; } = requestName;

    public Error? Error { get; } = error;
}

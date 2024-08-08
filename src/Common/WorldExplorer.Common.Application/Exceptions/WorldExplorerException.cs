using WorldExplorer.Common.Domain;

namespace WorldExplorer.Common.Application.Exceptions;

public sealed class WorldExplorerException : Exception
{
    public WorldExplorerException(string requestName, Error? error = default, Exception? innerException = default)
        : base("Application exception", innerException)
    {
        RequestName = requestName;
        Error = error;
    }

    public string RequestName { get; }

    public Error? Error { get; }
}

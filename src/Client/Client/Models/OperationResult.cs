namespace Client.Models;

public class OperationResult<T> where T : new()
{
	public T Result { get; init; } = new();

	public StatusCode StatusCode { get; init; }
}

public enum StatusCode
{
	Success,
	LocationInfoRequestPending,
	FailedResponse
}
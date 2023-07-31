namespace Client.Services.Auth;

using System.Diagnostics.CodeAnalysis;

public interface IOperationResult<out T> : IHasErrorResult
{
	T? Value { get; }

	[MemberNotNullWhen(true, nameof(Value))]
	new bool IsSuccessful { get; }
}

public record OperationResult<T> : HasErrorResult, IOperationResult<T>
{
	public T? Value { get; init; }

	[MemberNotNullWhen(true, nameof(Value))]
	public new bool IsSuccessful => !Errors.Any();
}
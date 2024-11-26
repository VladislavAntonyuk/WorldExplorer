namespace Client.Services.Auth;

using System.Diagnostics.CodeAnalysis;

public record OperationResult<T> : HasErrorResult, IOperationResult<T>
{
	public T? Value { get; init; }

	[MemberNotNullWhen(true, nameof(Value))]
	public new bool IsSuccessful => !Errors.Any();
}
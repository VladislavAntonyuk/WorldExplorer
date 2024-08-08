namespace Client.Services.Auth;

using System.Diagnostics.CodeAnalysis;

public interface IOperationResult<out T> : IHasErrorResult
{
	T? Value { get; }

	[MemberNotNullWhen(true, nameof(Value))]
	new bool IsSuccessful { get; }
}
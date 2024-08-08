namespace Client.Services.Auth;

public interface IHasErrorResult
{
	bool IsSuccessful { get; }

	ICollection<Error> Errors { get; }

	void AddError(string description, int errorCode);
	void AddError(string description);
}
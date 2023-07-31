namespace Client.Services.Auth;

public record Error(string Description, int Code = 0);

public interface IHasErrorResult
{
	bool IsSuccessful { get; }

	ICollection<Error> Errors { get; }

	void AddError(string description, int errorCode);
	void AddError(string description);
}

public record HasErrorResult : IHasErrorResult
{
	public bool IsSuccessful => !Errors.Any();
	public ICollection<Error> Errors { get; } = new List<Error>();

	public void AddError(string description, int errorCode)
	{
		AddError(new Error(description, errorCode));
	}

	public void AddError(string description)
	{
		AddError(description, 0);
	}

	public void AddError(Error error)
	{
		Errors.Add(error);
	}
}
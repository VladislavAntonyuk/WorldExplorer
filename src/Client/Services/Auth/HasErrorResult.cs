namespace Client.Services.Auth;

public record HasErrorResult : IHasErrorResult
{
	public bool IsSuccessful => Errors.Count == 0;
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
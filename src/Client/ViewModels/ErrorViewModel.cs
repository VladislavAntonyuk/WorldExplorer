namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Enums;
using Framework;

public partial class ErrorViewModel : BaseViewModel, IQueryAttributable
{
	private readonly Dictionary<ErrorCode, string> errors = new();

	[ObservableProperty]
	private string? errorMessage;

	public ErrorViewModel()
	{
		errors.Add(ErrorCode.NoInternet, "No internet");
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		Enum.TryParse<ErrorCode>(query["errorCode"].ToString(), true, out var errorCode);
		ErrorMessage = errors[errorCode];
	}
}
namespace Client.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using Framework;
using Models.Enums;
using Resources.Localization;

public partial class ErrorViewModel : BaseViewModel, IQueryAttributable
{
	private readonly Dictionary<ErrorCode, string> errors = [];

	[ObservableProperty]
	public partial ErrorCode? Code { get; private set; }

	[ObservableProperty]
	public partial string? Message { get; private set; }

	public ErrorViewModel()
	{
		errors.Add(ErrorCode.NoInternet, Localization.NoInternet);
	}

	public void ApplyQueryAttributes(IDictionary<string, object> query)
	{
		Enum.TryParse<ErrorCode>(query["errorCode"].ToString(), true, out var errorCode);
		Code = errorCode;
		Message = errors[errorCode];
	}
}
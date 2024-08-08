namespace Client.Views;

using Syncfusion.Maui.Popup;
using ViewModels;

public partial class PlaceDetailsView
{
	public PlaceDetailsView(PlaceDetailsViewModel placeDetailsViewModel) : base(placeDetailsViewModel)
	{
		InitializeComponent();
	}

	public SfPopup? Popup { get; set; }

	private void OpenArClicked(object? sender, EventArgs e)
	{
		Popup?.Dismiss();
		ViewModel.ArCommand.Execute(null);
	}
}
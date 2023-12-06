namespace Client.Views;

using Controls;
using Framework;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ViewModels;

public partial class ExplorerPage : BaseContentPage<ExplorerViewModel>
{
	private readonly IDispatcher dispatcher;

	public ExplorerPage(ExplorerViewModel viewModel, IDispatcher dispatcher) : base(viewModel)
	{
		this.dispatcher = dispatcher;
		viewModel.LocationChanged += OnLocationChanged;
		InitializeComponent();
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		if (!e.MoveToRegion)
		{
			return;
		}

		dispatcher.Dispatch(() =>
		{
			Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(1)));
		});
	}

	private void Pin_OnMarkerClicked(object? sender, PinClickedEventArgs e)
	{
		if (sender is not WorldExplorerPin pin)
		{
			return;
		}

		e.HideInfoWindow = true;
		var placeDetailsViewModel = IPlatformApplication.Current?.Services.GetRequiredService<PlaceDetailsViewModel>();
		if (placeDetailsViewModel is null)
		{
			return;
		}

		placeDetailsViewModel.ApplyQueryAttributes(new Dictionary<string, object>
		{
			{
				"place", pin.PlaceId
			}
		});

		var placeDetailsView = new PlaceDetailsView(placeDetailsViewModel);
		var bottomSheet = this.ShowBottomSheet(placeDetailsView, true);
		placeDetailsView.BottomSheet = bottomSheet;
	}

	private void HelpMenuItemClicked(object? sender, EventArgs e)
	{
		ViewModel.HelpCommand.Execute(null);
	}

	private void AboutMenuItemClicked(object? sender, EventArgs e)
	{
		ViewModel.AboutCommand.Execute(null);
	}

	private void ToggleUserLocationClicked(object? sender, EventArgs e)
	{
		ViewModel.ToggleUserLocationCommand.Execute(null);
	}
}
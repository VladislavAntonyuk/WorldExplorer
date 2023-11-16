namespace Client.Views;

using Framework;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Shared.Models;
using ViewModels;

public partial class ExplorerPage : BaseContentPage<ExplorerViewModel>
{
	private readonly PlaceDetailsViewModel placeDetailsViewModel;
	private readonly IDispatcher dispatcher;

	public ExplorerPage(ExplorerViewModel viewModel, PlaceDetailsViewModel placeDetailsViewModel, IDispatcher dispatcher) : base(viewModel)
	{
		this.placeDetailsViewModel = placeDetailsViewModel;
		this.dispatcher = dispatcher;
		viewModel.LocationChanged += OnLocationChanged;
		InitializeComponent();
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		dispatcher.Dispatch(() =>
		{
			Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(1)));
		});
	}

	private async void Pin_OnMarkerClicked(object? sender, PinClickedEventArgs e)
	{
		if (sender is not Pin pin)
		{
			return;
		}

		e.HideInfoWindow = true;
		placeDetailsViewModel.ApplyQueryAttributes(new Dictionary<string, object>
		{
			{
				"place", new Place
				{
					Name = pin.Label,
					Location = new Location(pin.Location.Latitude, pin.Location.Longitude)
				}
			}
		});

		await placeDetailsViewModel.InitializeAsync();
		if (placeDetailsViewModel.Place != Place.Default)
		{
			var placeDetailsView = new PlaceDetailsView(placeDetailsViewModel);
			var bottomSheet = this.ShowBottomSheet(placeDetailsView, true);
			placeDetailsView.BottomSheet = bottomSheet;
		}
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
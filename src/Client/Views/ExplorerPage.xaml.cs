namespace Client.Views;

using Framework;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Shared.Models;
using ViewModels;

public partial class ExplorerPage : BaseContentPage<ExplorerViewModel>
{
	private readonly PlaceDetailsViewModel placeDetailsViewModel;

	public ExplorerPage(ExplorerViewModel viewModel, PlaceDetailsViewModel placeDetailsViewModel) : base(viewModel)
	{
		this.placeDetailsViewModel = placeDetailsViewModel;
		viewModel.LocationChanged += OnLocationChanged;
		InitializeComponent();
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		Map.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(1)));
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
		var placeDetailsView = new PlaceDetailsView(placeDetailsViewModel);
		this.ShowBottomSheet(placeDetailsView, true);
	}
}
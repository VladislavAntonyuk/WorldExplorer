namespace Client.Views;

using Controls;
using Framework;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Syncfusion.Maui.Popup;
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

		var placeDetailsView = new PlaceDetailsView(placeDetailsViewModel);
		placeDetailsViewModel.ApplyQueryAttributes(new Dictionary<string, object>
		{
			{
				"place", pin.PlaceId
			}
		});
		var popup = new SfPopup
		{
			ContentTemplate = new DataTemplate(() => placeDetailsView),
			StaysOpen = true,
			ShowCloseButton = true,
			Parent = this,
			AnimationMode = PopupAnimationMode.Fade,
			AutoSizeMode = PopupAutoSizeMode.Both,
			HeaderTitle = pin.Label
		};
		popup.Show();
		placeDetailsView.Popup = popup;
	}
}
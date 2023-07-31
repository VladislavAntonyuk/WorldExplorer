namespace Client.Views;

using Framework;
using ViewModels;

public partial class PlaceDetailsView : BaseContentView<PlaceDetailsViewModel>
{
	public PlaceDetailsView(PlaceDetailsViewModel placeDetailsViewModel) : base(placeDetailsViewModel)
	{
		InitializeComponent();
	}
}
namespace Client.Views;

#if ANDROID
using BottomSheet = Google.Android.Material.BottomSheet.BottomSheetDialog;
#elif IOS
using BottomSheet = UIKit.UIViewController;
#elif MACCATALYST
using BottomSheet = Syncfusion.Maui.Popup.SfPopup;
#elif WINDOWS
using BottomSheet = Syncfusion.Maui.Popup.SfPopup;
#elif TIZEN
using BottomSheet = Tizen.UIExtensions.NUI.Popup;
#endif
using ViewModels;

public partial class PlaceDetailsView
{
	public PlaceDetailsView(PlaceDetailsViewModel placeDetailsViewModel) : base(placeDetailsViewModel)
	{
		InitializeComponent();
	}

	public BottomSheet? BottomSheet { get; set; }

	private void OpenArClicked(object? sender, EventArgs e)
	{
		BottomSheet?.CloseBottomSheet();
		ViewModel.ArCommand.Execute(null);
	}
}
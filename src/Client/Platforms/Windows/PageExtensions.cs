namespace Client;

using Syncfusion.Maui.Popup;

public static class PageExtensions
{
	public static SfPopup ShowBottomSheet(this Page page, IView bottomSheetContent, bool dimDismiss)
	{
		var popup = new SfPopup
		{
			ContentTemplate = new DataTemplate(() => bottomSheetContent),
			StaysOpen = !dimDismiss,
			ShowCloseButton = false,
			Parent = page,
			AnimationMode = PopupAnimationMode.Fade,
			AutoSizeMode = PopupAutoSizeMode.Both,
			ShowHeader = false
		};
		popup.Show();
		return popup;
	}

	public static void CloseBottomSheet(this SfPopup bottomSheet)
	{
		bottomSheet.IsOpen = false;
	}
}
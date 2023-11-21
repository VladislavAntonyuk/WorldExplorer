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
			ShowCloseButton = true,
			Parent = page,
			AutoSizeMode = PopupAutoSizeMode.Both,
			AnimationMode = PopupAnimationMode.Fade,
			HeaderTitle = string.Empty
		};
		popup.Show();
		return popup;
	}

	public static void CloseBottomSheet(this SfPopup bottomSheet)
	{
		bottomSheet.IsOpen = false;
	}
}
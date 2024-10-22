namespace Client.Framework;

using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

public abstract class BasePopup<T> : BaseContentPage<T>
	where T : BasePopupViewModel
{
	private readonly T basePopupViewModel;

	protected BasePopup(T basePopupViewModel) : base(basePopupViewModel)
	{
		this.basePopupViewModel = basePopupViewModel;
		BackgroundColor = Color.FromRgba(0, 0, 0, 0.4); // https://rgbacolorpicker.com/rgba-to-hex
		Shell.SetPresentationMode(this, PresentationMode.ModalNotAnimated);
		On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.OverFullScreen);
	}

	protected override bool OnBackButtonPressed()
	{
		basePopupViewModel.ResetState();
		basePopupViewModel.ClosePopup().ConfigureAwait(false);
		return true;
	}
}
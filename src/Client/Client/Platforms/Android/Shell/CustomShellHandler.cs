namespace Client;

using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

internal class CustomShellHandler : ShellRenderer
{
	protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
	{
		return new CustomShellBottomNavViewAppearanceTracker(this, shellItem.CurrentItem);
	}

	protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
	{
		return new CustomShellToolbarAppearanceTracker(this);
	}

	protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
	{
		return new CustomShellItemRenderer(this);
	}

	protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
	{
		return new CustomShellSectionRenderer(this);
	}
}
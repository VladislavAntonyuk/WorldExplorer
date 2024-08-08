#pragma warning disable CS0169
#pragma warning disable CS0414
namespace Client.Controls;

using System.Windows.Input;
using Maui.BindableProperty.Generator.Core;

public partial class CustomTabBar : TabBar
{
	[AutoBindable]
	private Color? centerViewBackgroundColor;

	[AutoBindable]
	private ICommand? centerViewCommand;

	[AutoBindable]
	private ImageSource? centerViewImageSource;

	[AutoBindable]
	private bool centerViewVisible;
}
namespace Client.Views;

using Controls;
using Framework;
using Syncfusion.Maui.Popup;
using ViewModels;
using WorldExplorer.Client.Map.WorldExplorerMap;

public partial class ExplorerPage : BaseContentPage<ExplorerViewModel>
{
	public ExplorerPage(ExplorerViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
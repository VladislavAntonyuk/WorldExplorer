namespace Client.Views;

using Framework;
using ViewModels;

public partial class ExplorerPage : BaseContentPage<ExplorerViewModel>
{
	public ExplorerPage(ExplorerViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
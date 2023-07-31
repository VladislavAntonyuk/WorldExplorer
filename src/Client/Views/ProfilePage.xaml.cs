namespace Client.Views;

using Framework;
using ViewModels;

public partial class ProfilePage : BaseContentPage<ProfileViewModel>
{
	public ProfilePage(ProfileViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
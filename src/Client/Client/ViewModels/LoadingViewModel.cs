﻿namespace Client.ViewModels;

using CommunityToolkit.Mvvm.Messaging;
using Framework;
using Models.Enums;
using Services.API;
using Services.Auth;
using Services.Navigation;

public class LoadingViewModel(INavigationService navigation, IUsersApi usersApi, ICurrentUserService currentUserService) : BaseViewModel
{
	public override async Task InitializeAsync()
	{
		var currentUser = currentUserService.GetCurrentUser();
		if (currentUser is null)
		{
			var user = await usersApi.GetCurrentUser(CancellationToken.None);
			if (!user.IsSuccessful)
			{
				if (user.Error.ReasonPhrase == "Site Disabled")
				{
					await navigation.NavigateAsync<ErrorViewModel, ErrorViewModel>(new Dictionary<string, object>
					{
						{"errorCode", ErrorCode.SiteDisabled}
					});
					return;
				}

				await navigation.NavigateAsync<LoginViewModel, ErrorViewModel>();
				return;
			}

			currentUser = user.Content;
		}

		WeakReferenceMessenger.Default.Send(new UserAuthenticatedEvent(currentUser));
		await navigation.NavigateAsync<ExplorerViewModel, ErrorViewModel>();
	}
}
namespace Client.Services.Auth;

using API;
using CommunityToolkit.Mvvm.Messaging;
using Shared.Models;

internal class CurrentUserService : ICurrentUserService, IRecipient<UserAuthenticatedEvent>
{
	private User? currentUser;

	public CurrentUserService()
	{
		WeakReferenceMessenger.Default.Register(this);
	}

	public User? GetCurrentUser()
	{
		return currentUser;
	}

	public void Receive(UserAuthenticatedEvent message)
	{
		currentUser = message.User;
	}
}

public interface ICurrentUserService
{
	User? GetCurrentUser();
}

public record UserAuthenticatedEvent(User? User);
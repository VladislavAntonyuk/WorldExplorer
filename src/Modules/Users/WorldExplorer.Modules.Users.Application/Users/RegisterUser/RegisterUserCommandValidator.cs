namespace WorldExplorer.Modules.Users.Application.Users.RegisterUser;

using FluentValidation;

internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
	public RegisterUserCommandValidator()
	{
		RuleFor(c => c.ProviderId).NotEmpty();
	}
}
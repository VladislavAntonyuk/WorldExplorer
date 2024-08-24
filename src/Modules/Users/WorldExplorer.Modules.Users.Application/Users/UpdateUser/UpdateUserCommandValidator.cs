namespace WorldExplorer.Modules.Users.Application.Users.UpdateUser;

using FluentValidation;

internal sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
	public UpdateUserCommandValidator()
	{
		RuleFor(c => c.UserId).NotEmpty();
	}
}
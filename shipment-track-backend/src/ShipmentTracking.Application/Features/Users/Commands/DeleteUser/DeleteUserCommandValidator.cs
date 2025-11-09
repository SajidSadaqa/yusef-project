using FluentValidation;

namespace ShipmentTracking.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Validator for <see cref="DeleteUserCommand"/>.
/// </summary>
public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}

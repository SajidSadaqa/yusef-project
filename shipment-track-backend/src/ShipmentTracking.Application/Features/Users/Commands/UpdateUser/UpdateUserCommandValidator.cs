using FluentValidation;

namespace ShipmentTracking.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Validator for <see cref="UpdateUserCommand"/>.
/// </summary>
public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] ValidRoles = { "Admin", "Staff", "Customer" };
    private static readonly string[] ValidStatuses = { "Active", "Inactive", "Suspended" };

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.LastName));

        RuleFor(x => x.Role)
            .Must(role => ValidRoles.Contains(role!))
            .WithMessage($"Role must be one of: {string.Join(", ", ValidRoles)}")
            .When(x => !string.IsNullOrWhiteSpace(x.Role));

        RuleFor(x => x.Status)
            .Must(status => ValidStatuses.Contains(status!))
            .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }
}

using System.Linq;
using FluentValidation;

namespace ShipmentTracking.Application.Features.Users.Commands.UpdateRoles;

/// <summary>
/// Validator for <see cref="UpdateUserRolesCommand"/>.
/// </summary>
public sealed class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.Roles)
            .NotNull()
            .Must(roles => roles.Count > 0)
            .WithMessage("At least one role must be provided.")
            .Must(roles => roles.All(role => !string.IsNullOrWhiteSpace(role)))
            .WithMessage("Roles cannot contain empty values.");
    }
}

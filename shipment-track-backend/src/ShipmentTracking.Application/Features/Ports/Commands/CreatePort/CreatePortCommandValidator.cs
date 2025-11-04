using FluentValidation;
using ShipmentTracking.Application.Features.Ports.Dto;

namespace ShipmentTracking.Application.Features.Ports.Commands.CreatePort;

/// <summary>
/// Validator for <see cref="CreatePortCommand"/>.
/// </summary>
public sealed class CreatePortCommandValidator : AbstractValidator<CreatePortCommand>
{
    public CreatePortCommandValidator()
    {
        RuleFor(x => x.Payload).NotNull();

        When(x => x.Payload != null, () =>
        {
            RuleFor(x => x.Payload.Name)
                .NotEmpty().WithMessage("Port name is required.")
                .MaximumLength(200).WithMessage("Port name cannot exceed 200 characters.");

            RuleFor(x => x.Payload.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

            RuleFor(x => x.Payload.City)
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Payload.City));

            RuleFor(x => x.Payload.Code)
                .MaximumLength(5).WithMessage("Port code cannot exceed 5 characters.")
                .Matches("^[A-Z]{5}$").WithMessage("Port code must be 5 uppercase letters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Payload.Code));
        });
    }
}

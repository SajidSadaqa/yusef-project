using FluentValidation;

namespace ShipmentTracking.Application.Features.Ports.Commands.UpdatePort;

public sealed class UpdatePortCommandValidator : AbstractValidator<UpdatePortCommand>
{
    public UpdatePortCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Port ID is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .WithMessage("Port name is required.")
            .MaximumLength(200)
            .WithMessage("Port name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .MaximumLength(100)
            .WithMessage("Country must not exceed 100 characters.");

        RuleFor(x => x.Dto.City)
            .MaximumLength(100)
            .WithMessage("City must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Dto.City));
    }
}

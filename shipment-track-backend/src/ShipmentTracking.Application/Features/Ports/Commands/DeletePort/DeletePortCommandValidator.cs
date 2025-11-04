using FluentValidation;

namespace ShipmentTracking.Application.Features.Ports.Commands.DeletePort;

public sealed class DeletePortCommandValidator : AbstractValidator<DeletePortCommand>
{
    public DeletePortCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Port ID is required.");
    }
}

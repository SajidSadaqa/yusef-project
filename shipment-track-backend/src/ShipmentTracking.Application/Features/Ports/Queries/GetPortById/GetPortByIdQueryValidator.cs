using FluentValidation;

namespace ShipmentTracking.Application.Features.Ports.Queries.GetPortById;

public sealed class GetPortByIdQueryValidator : AbstractValidator<GetPortByIdQuery>
{
    public GetPortByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Port ID is required.");
    }
}

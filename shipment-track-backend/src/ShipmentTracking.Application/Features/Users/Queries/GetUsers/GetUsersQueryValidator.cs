using FluentValidation;

namespace ShipmentTracking.Application.Features.Users.Queries.GetUsers;

/// <summary>
/// Validator for <see cref="GetUsersQuery"/>.
/// </summary>
public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SearchTerm)
            .MaximumLength(128);
    }
}

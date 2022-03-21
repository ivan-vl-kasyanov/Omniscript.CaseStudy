using FluentValidation;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.GetCustomers
{
    internal sealed class GetCustomersCommandValidator : AbstractValidator<GetCustomersCommand>
    {
        public GetCustomersCommandValidator()
        {
            RuleFor(command => command.Skip)
                .GreaterThanOrEqualTo(0)
                .When(command => command.Skip.HasValue)
                .WithMessage("Skip cannot be less than 0.");

            RuleFor(command => command.Take)
                .GreaterThan(0)
                .When(command => command.Take.HasValue)
                .WithMessage("Take must to be greater than 0.");
        }
    }
}
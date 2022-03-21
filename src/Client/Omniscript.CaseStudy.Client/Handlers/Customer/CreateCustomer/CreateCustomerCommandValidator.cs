using FluentValidation;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.CreateCustomer
{
    internal sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(command => command.NewCustomer)
                .NotNull()
                .WithMessage("NewCustomer cannot be NULL.");
            RuleFor(command => command.NewCustomer)
                .SetValidator(new NewCustomerModelValidator())
                .When(newCustomer => newCustomer != null);
        }
    }
}
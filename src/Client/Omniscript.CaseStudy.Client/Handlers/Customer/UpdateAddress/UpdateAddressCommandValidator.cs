using FluentValidation;

using Omniscript.CaseStudy.Client.Handlers.Common;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.UpdateAddress
{
    internal sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
    {
        public UpdateAddressCommandValidator()
        {
            RuleFor(command => command.CustomerId)
                .GreaterThan(0L)
                .WithMessage("CustomerId must be greater than 0.");

            RuleFor(command => command.Address)
                .NotNull()
                .WithMessage("Address cannot be NULL.");
            RuleFor(command => command.Address)
                .SetValidator(new AddressModelValidator())
                .When(address => address != null);
        }
    }
}
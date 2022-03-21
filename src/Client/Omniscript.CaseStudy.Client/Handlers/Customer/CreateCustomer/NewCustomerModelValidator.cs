using System;

using FluentValidation;

using Omniscript.CaseStudy.Client.Handlers.Common;
using Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.CreateCustomer
{
    internal sealed class NewCustomerModelValidator : AbstractValidator<NewCustomerModel>
    {
        public NewCustomerModelValidator()
        {
            RuleFor(entity => entity.Email)
                .NotNull()
                .WithMessage("Email cannot be NULL.");
            RuleFor(entity => entity.Email)
                .Must(email => email.Trim() != String.Empty)
                .When(entity => entity.Email != null)
                .WithMessage("Email cannot be empty.");
            RuleFor(entity => entity.Email)
                .Must(email => email.Length >= 5)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Email))
                .WithMessage("Email is too short (< 5).");
            RuleFor(entity => entity.Email)
                .Must(email => email.Length <= 128)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Email))
                .WithMessage("Email is too long (> 128).");
            RuleFor(entity => entity.Email)
                .EmailAddress()
                .When(entity =>
                    (entity.Email != null) &&
                    (entity.Email.Length >= 5) &&
                    (entity.Email.Length <= 128))
                .WithMessage("Email is incorrect.");

            RuleFor(entity => entity.Address)
                .NotNull()
                .WithMessage("Address cannot be NULL.");
            RuleFor(entity => entity.Address)
                .SetValidator(new AddressModelValidator())
                .When(address => address != null);
        }
    }
}
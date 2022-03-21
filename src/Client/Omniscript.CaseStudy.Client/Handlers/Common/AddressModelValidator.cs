using System;

using FluentValidation;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Client.Handlers.Common
{
    internal sealed class AddressModelValidator : AbstractValidator<AddressModel>
    {
        public AddressModelValidator()
        {
            RuleFor(entity => entity.Country)
                .NotNull()
                .WithMessage("Country cannot be NULL.");
            RuleFor(entity => entity.Country)
                .Must(country => country.Trim() != String.Empty)
                .When(entity => entity.Country != null)
                .WithMessage("Country cannot be empty.");
            RuleFor(entity => entity.Country)
                .Must(country => country.Length >= 2)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Country))
                .WithMessage("Country is too short (< 2).");
            RuleFor(entity => entity.Country)
                .Must(country => country.Length <= 256)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Country))
                .WithMessage("Country is too long (> 256).");

            RuleFor(entity => entity.City)
                .NotNull()
                .WithMessage("City cannot be NULL.");
            RuleFor(entity => entity.City)
                .Must(city => city.Trim() != String.Empty)
                .When(entity => entity.City != null)
                .WithMessage("City cannot be empty.");
            RuleFor(entity => entity.City)
                .Must(city => city.Length >= 2)
                .When(entity => !String.IsNullOrWhiteSpace(entity.City))
                .WithMessage("City is too short (< 2).");
            RuleFor(entity => entity.City)
                .Must(city => city.Length <= 256)
                .When(entity => !String.IsNullOrWhiteSpace(entity.City))
                .WithMessage("City is too long (> 256).");

            RuleFor(entity => entity.Street)
                .NotNull()
                .WithMessage("Street cannot be NULL.");
            RuleFor(entity => entity.Street)
                .Must(street => street.Trim() != String.Empty)
                .When(entity => entity.Street != null)
                .WithMessage("Street cannot be empty.");
            RuleFor(entity => entity.Street)
                .Must(street => street.Length >= 2)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Street))
                .WithMessage("Street is too short (< 2).");
            RuleFor(entity => entity.Street)
                .Must(street => street.Length <= 256)
                .When(entity => !String.IsNullOrWhiteSpace(entity.Street))
                .WithMessage("Street is too long (> 256).");
        }
    }
}
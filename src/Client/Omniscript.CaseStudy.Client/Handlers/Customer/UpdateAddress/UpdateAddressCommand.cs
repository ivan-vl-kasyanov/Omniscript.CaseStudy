using System.Net;

using MediatR;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.UpdateAddress
{
    internal sealed class UpdateAddressCommand : IRequest<HttpStatusCode>
    {
        public long CustomerId { get; init; }

        public AddressModel Address { get; init; }

        public UpdateAddressCommand(
            long customerId,
            AddressModel address)
        {
            CustomerId = customerId;
            Address = address;
        }
    }
}
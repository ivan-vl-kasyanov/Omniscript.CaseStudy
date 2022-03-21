using System.Net;

using MediatR;

using Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.CreateCustomer
{
    internal sealed class CreateCustomerCommand : IRequest<HttpStatusCode>
    {
        public NewCustomerModel NewCustomer { get; init; }

        public CreateCustomerCommand(NewCustomerModel newCustomer)
        {
            NewCustomer = newCustomer;
        }
    }
}
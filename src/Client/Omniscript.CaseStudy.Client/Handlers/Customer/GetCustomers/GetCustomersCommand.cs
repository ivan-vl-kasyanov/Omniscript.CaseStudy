using System.Collections.Generic;
using System.Net;

using MediatR;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Client.Handlers.Customer.GetCustomers
{
    internal sealed class GetCustomersCommand
        : IRequest<(HttpStatusCode StatusCode, int CustomersTotalCount, IEnumerable<CustomerModel> Customers)>
    {
        public int? Skip { get; init; }

        public int? Take { get; init; }

        public GetCustomersCommand(
            int? skip,
            int? take)
        {
            Skip = skip;
            Take = take;
        }
    }
}
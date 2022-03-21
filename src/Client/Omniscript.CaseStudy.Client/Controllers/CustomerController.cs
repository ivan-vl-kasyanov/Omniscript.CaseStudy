using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Omniscript.CaseStudy.Client.Handlers.Customer.CreateCustomer;
using Omniscript.CaseStudy.Client.Handlers.Customer.GetCustomers;
using Omniscript.CaseStudy.Client.Handlers.Customer.UpdateAddress;
using Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer;
using Omniscript.CaseStudy.Server.Models.Customer.GetCustomers;
using Omniscript.CaseStudy.Server.Models.Customer.UpdateAddress;

using Swashbuckle.AspNetCore.Annotations;

namespace Omniscript.CaseStudy.Client.Controllers
{
    /// <summary>
    /// Customer operations controller.
    /// </summary>
    [ApiController,
     Route("[controller]"),
     Produces(MediaTypeNames.Application.Json)]
    public sealed class CustomerController : Controller
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor of the customer operations controller.
        /// </summary>
        /// <param name="mediator">Defines a mediator to encapsulate request/response and publishing interaction patterns.</param>
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Requesting customers.
        /// </summary>
        /// <param name="request">Request of customers.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Response for the request of customers.</returns>
        [SwaggerResponse(
            (int)HttpStatusCode.OK,
            "Response for the request of customers.",
            typeof(GetCustomersResponse))]
        [HttpGet("customers")]
        public async Task<GetCustomersResponse> GetCustomersAsync(
            [FromQuery] GetCustomersRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                var exceptionMessage = $"{nameof(request)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }

            var command = new GetCustomersCommand(
                request.Skip,
                request.Take);
            var result = await _mediator.Send(
                command,
                cancellationToken);
            var response = new GetCustomersResponse(
                result.Customers,
                result.CustomersTotalCount
            );
            Response.StatusCode = (int)result.StatusCode;

            return response;
        }

        /// <summary>
        /// Creating new customer.
        /// </summary>
        /// <param name="request">Request for customer creation.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Operation HTTP status code.</returns>
        [SwaggerResponse(
            (int)HttpStatusCode.NoContent,
            "Operation HTTP status code.",
            typeof(GetCustomersResponse))]
        [HttpPost("new-customer")]
        public async Task CreateCustomerAsync(
            [FromBody] CreateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                var exceptionMessage = $"{nameof(request)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }

            var command = new CreateCustomerCommand(request.NewCustomer);
            var result = await _mediator.Send(
                command,
                cancellationToken);
            Response.StatusCode = (int)result;
        }

        /// <summary>
        /// Updating customer's address.
        /// </summary>
        /// <param name="request">Request for updating customer's address.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns>Operation HTTP status code.</returns>
        [SwaggerResponse(
            (int)HttpStatusCode.NoContent,
            "Operation HTTP status code.",
            typeof(GetCustomersResponse))]
        [HttpPatch("update-address")]
        public async Task UpdateAddressAsync(
            [FromBody] UpdateAddressRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
            {
                var exceptionMessage = $"{nameof(request)} cannot be NULL.";

                throw new ArgumentNullException(exceptionMessage);
            }

            var command = new UpdateAddressCommand(
                request.CustomerId,
                request.Address);
            var result = await _mediator.Send(
                command,
                cancellationToken);
            Response.StatusCode = (int)result;
        }
    }
}
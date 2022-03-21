using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Server.Models.Customer.GetCustomers
{
    /// <summary>
    /// Response for the request of customers.
    /// </summary>
    public sealed class GetCustomersResponse
    {
        /// <summary>
        /// Acquired customers.
        /// </summary>
        [Required]
        public IEnumerable<CustomerModel> Customers { get; set; }

        /// <summary>
        /// Total amount of stored customers.
        /// </summary>
        [Required,
         Range(0, Int32.MaxValue)]
        public int CustomersTotalCount { get; set; }

        /// <summary>
        /// Constructor of the response for the request of customers.
        /// </summary>
        /// <param name="customers">Acquired customers.</param>
        /// <param name="customersTotalCount">Total amount of stored customers.</param>
        public GetCustomersResponse(
            [Required] IEnumerable<CustomerModel> customers,
            [Required, Range(0, Int32.MaxValue)] int customersTotalCount)
        {
            Customers = customers;
            CustomersTotalCount = customersTotalCount;
        }
    }
}
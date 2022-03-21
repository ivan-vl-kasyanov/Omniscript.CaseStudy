using System;
using System.ComponentModel.DataAnnotations;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Server.Models.Customer.UpdateAddress
{
    /// <summary>
    /// Request for updating customer's address.
    /// </summary>
    public sealed class UpdateAddressRequest
    {
        /// <summary>
        /// Customer's identifier.
        /// </summary>
        [Required,
         Range(1, UInt64.MaxValue)]
        public long CustomerId { get; set; }

        /// <summary>
        /// Customer's address instance.
        /// </summary>
        [Required]
        public AddressModel Address { get; set; }

        /// <summary>
        /// Constructor for the request for updating customer's address.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <param name="address">Customer's address instance.</param>
        public UpdateAddressRequest(
            long customerId,
            AddressModel address)
        {
            CustomerId = customerId;
            Address = address;
        }
    }
}
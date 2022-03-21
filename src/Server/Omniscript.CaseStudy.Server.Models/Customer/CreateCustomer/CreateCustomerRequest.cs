using System.ComponentModel.DataAnnotations;

namespace Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer
{
    /// <summary>
    /// Request for customer creation.
    /// </summary>
    public sealed class CreateCustomerRequest
    {
        /// <summary>
        /// New customer entity.
        /// </summary>
        [Required]
        public NewCustomerModel NewCustomer { get; set; }

        /// <summary>
        /// Constructor of the request for customer creation.
        /// </summary>
        /// <param name="newCustomer">New customer entity.</param>
        public CreateCustomerRequest([Required] NewCustomerModel newCustomer)
        {
            NewCustomer = newCustomer;
        }
    }
}
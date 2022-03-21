using System.ComponentModel.DataAnnotations;

using Omniscript.CaseStudy.Server.Models.Entities;

namespace Omniscript.CaseStudy.Server.Models.Customer.CreateCustomer
{
    /// <summary>
    /// Model of the new customer.
    /// </summary>
    public sealed class NewCustomerModel
    {
        /// <summary>
        /// E-mail address.
        /// </summary>
        [Required(AllowEmptyStrings = false),
         MinLength(5),
         MaxLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Customer's address instance.
        /// </summary>
        [Required]
        public AddressModel Address { get; set; }

        /// <summary>
        /// Constructor of the model of the customer.
        /// </summary>
        /// <param name="email">E-mail address.</param>
        /// <param name="address">Customer's address instance.</param>
        public NewCustomerModel(
            [Required(AllowEmptyStrings = false), MinLength(5), MaxLength(128)] string email,
            [Required] AddressModel address)
        {
            Email = email;
            Address = address;
        }
    }
}
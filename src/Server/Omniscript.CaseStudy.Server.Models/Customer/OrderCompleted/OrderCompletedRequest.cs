using System;
using System.ComponentModel.DataAnnotations;

namespace Omniscript.CaseStudy.Server.Models.Customer.OrderCompleted
{
    /// <summary>
    /// Request of the order completion.
    /// </summary>
    public sealed class OrderCompletedRequest
    {
        /// <summary>
        /// E-mail address.
        /// </summary>
        [Required(AllowEmptyStrings = false),
         MinLength(5),
         MaxLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Order creation timestamp.
        /// </summary>
        [Required]
        public DateTimeOffset OrderCreatedAt { get; set; }

        /// <summary>
        /// Constructor of the request of the order completion.
        /// </summary>
        /// <param name="email">E-mail address.</param>
        /// <param name="orderCreatedAt">Order creation timestamp.</param>
        public OrderCompletedRequest(
            [Required(AllowEmptyStrings = false), MinLength(5), MaxLength(128)] string email,
            [Required] DateTimeOffset orderCreatedAt)
        {
            Email = email;
            OrderCreatedAt = orderCreatedAt;
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Omniscript.CaseStudy.Server.Models.Entities
{
    /// <summary>
    /// Model of the customer.
    /// </summary>
    public sealed class CustomerModel
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        [Required,
         Range(1, UInt64.MaxValue)]
        public long Id { get; set; }

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
        /// Record created timestamp.
        /// </summary>
        [Required]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// An indication that the entity has been archived.
        /// </summary>
        [Required]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Purchased timestamp.
        /// </summary>
        public DateTimeOffset? PurchasedAt { get; set; }

        /// <summary>
        /// Constructor of the model of the customer.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="email">E-mail address.</param>
        /// <param name="address">Customer's address instance.</param>
        /// <param name="createdAt">Record created timestamp.</param>
        /// <param name="isArchived">An indication that the entity has been archived.</param>
        /// <param name="purchasedAt">Purchased timestamp.</param>
        public CustomerModel(
            [Required, Range(1, UInt64.MaxValue)] long id,
            [Required(AllowEmptyStrings = false), MinLength(5), MaxLength(128)] string email,
            [Required] AddressModel address,
            [Required] DateTimeOffset createdAt,
            [Required] bool isArchived,
            DateTimeOffset? purchasedAt)
        {
            Id = id;
            Email = email;
            Address = address;
            CreatedAt = createdAt;
            IsArchived = isArchived;
            PurchasedAt = purchasedAt;
        }
    }
}
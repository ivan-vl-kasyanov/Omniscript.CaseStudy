using System;

using SQLite;

namespace Omniscript.CaseStudy.Server.DataAccess.DAO
{
    /// <summary>
    /// Customer data access object.
    /// </summary>
    [Table("Customer")]
    internal sealed class CustomerDao
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        [PrimaryKey,
         AutoIncrement]
        public long Id { get; set; }

        /// <summary>
        /// E-mail address.
        /// </summary>
        [NotNull,
         Unique,
         MaxLength(128)]
        public string Email { get; set; }

        /// <summary>
        /// Record created timestamp.
        /// </summary>
        [NotNull]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// An indication that the entity has been archived.
        /// </summary>
        [NotNull]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Purchased timestamp.
        /// </summary>
        public DateTimeOffset? PurchasedAt { get; set; }

        /// <summary>
        /// Constructor of the customer data access object.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public CustomerDao() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
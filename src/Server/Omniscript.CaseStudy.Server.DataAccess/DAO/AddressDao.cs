using SQLite;

namespace Omniscript.CaseStudy.Server.DataAccess.DAO
{
    /// <summary>
    /// Address data access object.
    /// </summary>
    [Table("Address")]
    internal sealed class AddressDao
    {
        /// <summary>
        /// Customer's identifier.
        /// </summary>
        [PrimaryKey]
        public long CustomerId { get; set; }

        /// <summary>
        /// Country.
        /// </summary>
        [NotNull,
         MaxLength(256)]
        public string Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [NotNull,
         MaxLength(256)]
        public string City { get; set; }

        /// <summary>
        /// Street.
        /// </summary>
        [NotNull,
         MaxLength(256)]
        public string Street { get; set; }

        /// <summary>
        /// Constructor of the customer data access object.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public AddressDao() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
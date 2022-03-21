using System.ComponentModel.DataAnnotations;

namespace Omniscript.CaseStudy.Server.Models.Entities
{
    /// <summary>
    /// Model of the address.
    /// </summary>
    public sealed class AddressModel
    {
        /// <summary>
        /// Country.
        /// </summary>
        [Required(AllowEmptyStrings = false),
         MinLength(2),
         MaxLength(256)]
        public string Country { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [Required(AllowEmptyStrings = false),
         MinLength(2),
         MaxLength(256)]
        public string City { get; set; }

        /// <summary>
        /// Street.
        /// </summary>
        [Required(AllowEmptyStrings = false),
         MinLength(2),
         MaxLength(256)]
        public string Street { get; set; }

        /// <summary>
        /// Constructor of the model of the address.
        /// </summary>
        /// <param name="country">Country.</param>
        /// <param name="city">City.</param>
        /// <param name="street">Street.</param>
        public AddressModel(
            [Required(AllowEmptyStrings = false), MinLength(2), MaxLength(256)] string country,
            [Required(AllowEmptyStrings = false), MinLength(2), MaxLength(256)] string city,
            [Required(AllowEmptyStrings = false), MinLength(2), MaxLength(256)] string street)
        {
            Country = country;
            City = city;
            Street = street;
        }
    }
}
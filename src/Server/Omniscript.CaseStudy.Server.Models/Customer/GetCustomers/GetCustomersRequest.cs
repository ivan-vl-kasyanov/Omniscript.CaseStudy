using System;
using System.Collections.Generic;
using System.Linq;

namespace Omniscript.CaseStudy.Server.Models.Customer.GetCustomers
{
    /// <summary>
    /// Request of customers.
    /// </summary>
    public sealed class GetCustomersRequest
    {
        /// <summary>
        /// (Optional) Amount of skipped customers.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// (Optional) Amount of taken customers.
        /// </summary>
        public int? Take { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var values = new List<string>();

            if (Skip.HasValue)
            {
                values.Add($"{nameof(Skip)}={Skip.Value}");
            }

            if (Take.HasValue)
            {
                values.Add($"{nameof(Take)}={Take.Value}");
            }

            var result = values.Any()
                ? String.Join(
                    '&',
                    values)
                : String.Empty;

            return result;
        }
    }
}